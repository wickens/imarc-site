using ImarcAdmin.Config;
using ImarcAdmin.Models;
using Microsoft.Extensions.Options;

namespace ImarcAdmin.Services;

public sealed class GitContentRepository
{
    private readonly SemaphoreSlim _postListCacheLock = new(1, 1);
    private readonly IOptionsMonitor<AdminOptions> _optionsMonitor;
    private readonly GitCommandRunner _git;
    private readonly FrontMatterService _frontMatterService;
    private readonly TempUploadService _tempUploadService;
    private readonly SlugService _slugService;
    private IReadOnlyList<PostSummary>? _cachedPosts;
    private DateTimeOffset _postListCachedAtUtc = DateTimeOffset.MinValue;

    public GitContentRepository(
        IOptionsMonitor<AdminOptions> optionsMonitor,
        GitCommandRunner git,
        FrontMatterService frontMatterService,
        TempUploadService tempUploadService,
        SlugService slugService)
    {
        _optionsMonitor = optionsMonitor;
        _git = git;
        _frontMatterService = frontMatterService;
        _tempUploadService = tempUploadService;
        _slugService = slugService;
    }

    public Task<IReadOnlyList<PostSummary>> GetPostsAsync(CancellationToken cancellationToken)
        => GetPostsAsync(forceRefresh: false, cancellationToken);

    public async Task<IReadOnlyList<PostSummary>> GetPostsAsync(bool forceRefresh, CancellationToken cancellationToken)
    {
        if (!forceRefresh && TryGetCachedPosts(out var cachedPosts))
        {
            return cachedPosts;
        }

        await _postListCacheLock.WaitAsync(cancellationToken);
        try
        {
            if (!forceRefresh && TryGetCachedPosts(out cachedPosts))
            {
                return cachedPosts;
            }

            await PrepareRepositoryAsync(cancellationToken);
            var posts = await LoadPostSummariesAsync(cancellationToken);
            _cachedPosts = posts;
            _postListCachedAtUtc = DateTimeOffset.UtcNow;
            return posts;
        }
        finally
        {
            _postListCacheLock.Release();
        }
    }

    public async Task<EditablePost> StartNewPostAsync(CancellationToken cancellationToken)
    {
        var head = await PrepareRepositoryAsync(cancellationToken);
        return _frontMatterService.CreateNewEditablePost(head, _tempUploadService.CreateSession());
    }

    public async Task<EditablePost> LoadPostAsync(string postId, CancellationToken cancellationToken)
    {
        var head = await PrepareRepositoryAsync(cancellationToken);
        var repoPath = GetRepoPath();
        var relativePath = PostIdCodec.Decode(postId);
        var filePath = Path.Combine(repoPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Post not found at {relativePath}.");
        }

        var raw = await File.ReadAllTextAsync(filePath, cancellationToken);
        return _frontMatterService.ParseEditablePost(relativePath, postId, raw, head, _tempUploadService.CreateSession());
    }

    public async Task<PublishResult> PublishAsync(EditablePost post, CancellationToken cancellationToken)
    {
        var repoPath = GetRepoPath();
        var head = await PrepareRepositoryAsync(cancellationToken);
        if (!string.Equals(head, post.BaseCommit, StringComparison.Ordinal))
        {
            throw new RepoConflictException("The repository changed after you opened the editor. Reload the post before publishing.");
        }

        string relativePath;
        if (post.IsNew)
        {
            relativePath = ResolveNewPostPath(post.Slug, repoPath);
        }
        else if (!string.IsNullOrWhiteSpace(post.OriginalRelativePath))
        {
            relativePath = post.OriginalRelativePath;
        }
        else
        {
            throw new InvalidOperationException("Existing post is missing its original relative path.");
        }

        var fullPath = Path.Combine(repoPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? repoPath);
        await File.WriteAllTextAsync(fullPath, _frontMatterService.Serialize(post), cancellationToken);

        var uploadPaths = _tempUploadService.CommitSessionUploads(post.SessionId, repoPath);
        var addTargets = new List<string> { relativePath };
        addTargets.AddRange(uploadPaths);

        await _git.RunAsync($"config user.name {Quote(_optionsMonitor.CurrentValue.GitAuthorName)}", repoPath, cancellationToken);
        await _git.RunAsync($"config user.email {Quote(_optionsMonitor.CurrentValue.GitAuthorEmail)}", repoPath, cancellationToken);
        await _git.RunAsync("add " + string.Join(" ", addTargets.Select(Quote)), repoPath, cancellationToken);

        var commitMessage = post.IsNew ? $"Add post: {post.Title.Trim()}" : $"Update post: {post.Title.Trim()}";
        await _git.RunAsync($"commit -m {Quote(commitMessage)}", repoPath, cancellationToken);
        var pushedToRemote = await PushIfConfiguredAsync(repoPath, cancellationToken);
        var newHead = await _git.RunAsync("rev-parse HEAD", repoPath, cancellationToken);
        InvalidatePostsCache();

        return new PublishResult
        {
            PostId = PostIdCodec.Encode(relativePath),
            RelativePath = relativePath,
            HeadCommit = newHead,
            CommitMessage = commitMessage,
            PushedToRemote = pushedToRemote
        };
    }

    public async Task<DeleteResult> DeletePostAsync(EditablePost post, CancellationToken cancellationToken)
    {
        if (post.IsNew || string.IsNullOrWhiteSpace(post.OriginalRelativePath))
        {
            throw new InvalidOperationException("Only existing posts can be deleted.");
        }

        var repoPath = GetRepoPath();
        var head = await PrepareRepositoryAsync(cancellationToken);
        if (!string.Equals(head, post.BaseCommit, StringComparison.Ordinal))
        {
            throw new RepoConflictException("The repository changed after you opened the editor. Reload the post before deleting it.");
        }

        var fullPath = Path.Combine(repoPath, post.OriginalRelativePath.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Post not found at {post.OriginalRelativePath}.");
        }

        File.Delete(fullPath);
        _tempUploadService.DeleteSession(post.SessionId);

        await _git.RunAsync($"config user.name {Quote(_optionsMonitor.CurrentValue.GitAuthorName)}", repoPath, cancellationToken);
        await _git.RunAsync($"config user.email {Quote(_optionsMonitor.CurrentValue.GitAuthorEmail)}", repoPath, cancellationToken);
        await _git.RunAsync($"add {Quote(post.OriginalRelativePath)}", repoPath, cancellationToken);

        var commitMessage = $"Delete post: {post.Title.Trim()}";
        await _git.RunAsync($"commit -m {Quote(commitMessage)}", repoPath, cancellationToken);
        var pushedToRemote = await PushIfConfiguredAsync(repoPath, cancellationToken);
        var newHead = await _git.RunAsync("rev-parse HEAD", repoPath, cancellationToken);
        InvalidatePostsCache();

        return new DeleteResult
        {
            RelativePath = post.OriginalRelativePath,
            HeadCommit = newHead,
            CommitMessage = commitMessage,
            PushedToRemote = pushedToRemote
        };
    }

    private async Task<IReadOnlyList<PostSummary>> LoadPostSummariesAsync(CancellationToken cancellationToken)
    {
        var repoPath = GetRepoPath();
        var postsRoot = Path.Combine(repoPath, "src", "posts");
        var files = Directory.EnumerateFiles(postsRoot, "*.md", SearchOption.TopDirectoryOnly).ToArray();

        var posts = await Task.WhenAll(files.Select(async filePath =>
        {
            var relativePath = Path.GetRelativePath(repoPath, filePath).Replace('\\', '/');
            var raw = await File.ReadAllTextAsync(filePath, cancellationToken);
            var post = _frontMatterService.ParseEditablePost(relativePath, PostIdCodec.Encode(relativePath), raw, string.Empty, string.Empty);

            return new PostSummary
            {
                PostId = PostIdCodec.Encode(relativePath),
                RelativePath = relativePath,
                Title = post.Title,
                Slug = post.Slug,
                Permalink = post.Permalink,
                Excerpt = post.Excerpt,
                PublishedAt = post.OriginalTimestamp
            };
        }));

        return posts
            .OrderByDescending(post => post.PublishedAt ?? DateTimeOffset.MinValue)
            .ThenBy(post => post.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private async Task<string> PrepareRepositoryAsync(CancellationToken cancellationToken)
    {
        var repoPath = GetRepoPath();
        var remote = _optionsMonitor.CurrentValue.BlogRepoRemote;

        if (!Directory.Exists(Path.Combine(repoPath, ".git")))
        {
            var parent = Path.GetDirectoryName(repoPath);
            if (string.IsNullOrWhiteSpace(parent))
            {
                throw new InvalidOperationException("BlogRepoPath must include a parent directory.");
            }

            Directory.CreateDirectory(parent);
            if (Directory.Exists(repoPath) && Directory.EnumerateFileSystemEntries(repoPath).Any())
            {
                throw new InvalidOperationException($"BlogRepoPath '{repoPath}' already exists and is not an empty clone location.");
            }

            await _git.RunAsync($"clone --branch main --single-branch {Quote(remote)} {Quote(repoPath)}", parent, cancellationToken);
        }

        var status = await _git.RunAsync("status --porcelain", repoPath, cancellationToken);
        if (!string.IsNullOrWhiteSpace(status))
        {
            throw new InvalidOperationException("The managed blog clone has uncommitted changes. Clean that clone before using the admin UI.");
        }

        await _git.RunAsync("checkout main", repoPath, cancellationToken);
        await _git.RunAsync("fetch origin main", repoPath, cancellationToken);
        await _git.RunAsync("pull --ff-only origin main", repoPath, cancellationToken);
        return await _git.RunAsync("rev-parse HEAD", repoPath, cancellationToken);
    }

    private string ResolveNewPostPath(string slug, string repoPath)
    {
        var safeSlug = _slugService.Slugify(slug);
        var relativePath = $"src/posts/{safeSlug}.md";
        var fullPath = Path.Combine(repoPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            throw new InvalidOperationException($"A post file already exists for slug '{safeSlug}'. Choose a different slug.");
        }

        return relativePath;
    }

    private string GetRepoPath()
    {
        var repoPath = _optionsMonitor.CurrentValue.BlogRepoPath;
        if (string.IsNullOrWhiteSpace(repoPath))
        {
            throw new InvalidOperationException("Admin:BlogRepoPath is not configured.");
        }

        return repoPath;
    }

    private static string Quote(string value)
        => $"\"{value.Replace("\"", "\\\"", StringComparison.Ordinal)}\"";

    private async Task<bool> PushIfConfiguredAsync(string repoPath, CancellationToken cancellationToken)
    {
        if (!_optionsMonitor.CurrentValue.PushChangesToRemote)
        {
            return false;
        }

        await _git.RunAsync("push origin main", repoPath, cancellationToken);
        return true;
    }

    private bool TryGetCachedPosts(out IReadOnlyList<PostSummary> posts)
    {
        var cacheLifetime = TimeSpan.FromSeconds(Math.Max(0, _optionsMonitor.CurrentValue.PostListCacheSeconds));
        if (_cachedPosts is null || cacheLifetime <= TimeSpan.Zero || DateTimeOffset.UtcNow - _postListCachedAtUtc >= cacheLifetime)
        {
            posts = Array.Empty<PostSummary>();
            return false;
        }

        posts = _cachedPosts;
        return true;
    }

    private void InvalidatePostsCache()
    {
        _cachedPosts = null;
        _postListCachedAtUtc = DateTimeOffset.MinValue;
    }
}
