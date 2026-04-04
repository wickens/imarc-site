using System.Collections.Concurrent;
using ImarcAdmin.Config;
using ImarcAdmin.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace ImarcAdmin.Services;

public sealed class TempUploadService : BackgroundService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private readonly ConcurrentDictionary<string, List<UploadedImageDescriptor>> _uploads = new();
    private readonly ConcurrentDictionary<string, DateTimeOffset> _sessionActivity = new();
    private readonly IOptionsMonitor<AdminOptions> _optionsMonitor;
    private readonly SlugService _slugService;
    private readonly ILogger<TempUploadService> _logger;

    public TempUploadService(IOptionsMonitor<AdminOptions> optionsMonitor, SlugService slugService, ILogger<TempUploadService> logger)
    {
        _optionsMonitor = optionsMonitor;
        _slugService = slugService;
        _logger = logger;
    }

    public string CreateSession()
    {
        var sessionId = Guid.NewGuid().ToString("N");
        TouchSession(sessionId);
        return sessionId;
    }

    public IReadOnlyList<UploadedImageDescriptor> GetUploads(string sessionId)
        => _uploads.TryGetValue(sessionId, out var items)
            ? items.OrderByDescending(item => item.UploadedAtUtc).ToList()
            : Array.Empty<UploadedImageDescriptor>();

    public UploadedImageDescriptor? FindUpload(string sessionId, string uploadId)
        => GetUploads(sessionId).FirstOrDefault(item => item.Id == uploadId);

    public async Task<UploadedImageDescriptor> SaveUploadAsync(string sessionId, IBrowserFile file, string publishDate, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(file.Name);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Only JPG, JPEG, PNG, WEBP and GIF images are supported.");
        }

        var options = _optionsMonitor.CurrentValue;
        var tempRoot = GetTempRoot();
        Directory.CreateDirectory(tempRoot);
        var sessionRoot = Path.Combine(tempRoot, sessionId);
        Directory.CreateDirectory(sessionRoot);

        var parsedDate = DateTime.TryParse(publishDate, out var date)
            ? date
            : DateTime.UtcNow;
        var subfolder = $"{parsedDate:yyyy}/{parsedDate:MM}";
        var slug = _slugService.Slugify(Path.GetFileNameWithoutExtension(file.Name));
        var fileName = ResolveUniqueName(slug, extension, options.BlogRepoPath, subfolder);
        var relativeUploadPath = $"uploads/{subfolder}/{fileName}";
        var uploadId = Guid.NewGuid().ToString("N");
        var tempFilePath = Path.Combine(sessionRoot, $"{uploadId}{extension.ToLowerInvariant()}");

        await using (var target = File.Create(tempFilePath))
        await using (var input = file.OpenReadStream(options.MaxUploadSizeBytes, cancellationToken))
        {
            await input.CopyToAsync(target, cancellationToken);
        }

        var descriptor = new UploadedImageDescriptor
        {
            Id = uploadId,
            SessionId = sessionId,
            OriginalFileName = file.Name,
            RelativeUploadPath = relativeUploadPath,
            PublicUrl = $"/wp-content/{relativeUploadPath}",
            MarkdownSnippet = $"![](/wp-content/{relativeUploadPath})",
            PreviewUrl = $"/temp-uploads/{sessionId}/{uploadId}",
            TempFilePath = tempFilePath,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
            UploadedAtUtc = DateTimeOffset.UtcNow
        };

        _uploads.AddOrUpdate(sessionId, _ => new List<UploadedImageDescriptor> { descriptor }, (_, existing) =>
        {
            existing.Add(descriptor);
            return existing;
        });

        TouchSession(sessionId);
        return descriptor;
    }

    public IReadOnlyList<string> CommitSessionUploads(string sessionId, string repoPath)
    {
        if (!_uploads.TryGetValue(sessionId, out var items) || items.Count == 0)
        {
            return Array.Empty<string>();
        }

        var committed = new List<string>();
        foreach (var upload in items)
        {
            var destination = Path.Combine(repoPath, upload.RelativeUploadPath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(destination) ?? repoPath);
            File.Move(upload.TempFilePath, destination, overwrite: false);
            committed.Add(upload.RelativeUploadPath);
        }

        DeleteSession(sessionId);
        return committed;
    }

    public void DeleteSession(string sessionId)
    {
        _uploads.TryRemove(sessionId, out _);
        _sessionActivity.TryRemove(sessionId, out _);

        var sessionRoot = Path.Combine(GetTempRoot(), sessionId);
        if (Directory.Exists(sessionRoot))
        {
            Directory.Delete(sessionRoot, recursive: true);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        CleanupExpiredSessions();

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = TimeSpan.FromMinutes(Math.Max(5, _optionsMonitor.CurrentValue.TempCleanupIntervalMinutes));
            await Task.Delay(delay, stoppingToken);
            CleanupExpiredSessions();
        }
    }

    private void CleanupExpiredSessions()
    {
        var now = DateTimeOffset.UtcNow;
        var expiry = TimeSpan.FromHours(Math.Max(1, _optionsMonitor.CurrentValue.TempSessionLifetimeHours));

        foreach (var item in _sessionActivity.ToArray())
        {
            if (now - item.Value > expiry)
            {
                _logger.LogInformation("Cleaning stale upload session {SessionId}", item.Key);
                DeleteSession(item.Key);
            }
        }
    }

    private string GetTempRoot()
    {
        var tempPath = _optionsMonitor.CurrentValue.TempPath;
        if (string.IsNullOrWhiteSpace(tempPath))
        {
            tempPath = Path.Combine(Path.GetTempPath(), "imarc-admin-temp");
        }

        return tempPath;
    }

    private void TouchSession(string sessionId)
        => _sessionActivity[sessionId] = DateTimeOffset.UtcNow;

    private string ResolveUniqueName(string slug, string extension, string repoPath, string subfolder)
    {
        var safeSlug = string.IsNullOrWhiteSpace(slug) ? "image" : slug;
        var normalizedExtension = extension.ToLowerInvariant();
        var fileName = $"{safeSlug}{normalizedExtension}";

        var reserved = new HashSet<string>(
            _uploads.Values
                .SelectMany(items => items)
                .Where(item => item.RelativeUploadPath.StartsWith($"uploads/{subfolder}/", StringComparison.Ordinal))
                .Select(item => Path.GetFileName(item.RelativeUploadPath)),
            StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(repoPath))
        {
            var inMemorySuffix = 2;
            while (reserved.Contains(fileName))
            {
                fileName = $"{safeSlug}-{inMemorySuffix}{normalizedExtension}";
                inMemorySuffix++;
            }

            return fileName;
        }

        var directory = Path.Combine(repoPath, "uploads", subfolder.Replace('/', Path.DirectorySeparatorChar));
        var suffix = 2;

        while (reserved.Contains(fileName) || File.Exists(Path.Combine(directory, fileName)))
        {
            fileName = $"{safeSlug}-{suffix}{normalizedExtension}";
            suffix++;
        }

        return fileName;
    }
}
