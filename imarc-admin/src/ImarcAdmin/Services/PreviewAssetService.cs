using ImarcAdmin.Config;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace ImarcAdmin.Services;

public sealed class PreviewAssetService
{
    private readonly IOptionsMonitor<AdminOptions> _optionsMonitor;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

    public PreviewAssetService(IOptionsMonitor<AdminOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    public bool TryResolveWpContentAsset(string assetPath, out string filePath, out string contentType)
        => TryResolveAssetPath(assetPath, out filePath, out contentType);

    public bool TryResolveStaticAsset(string assetPath, out string filePath, out string contentType)
        => TryResolveAssetPath(Path.Combine("static", assetPath).Replace('\\', '/'), out filePath, out contentType);

    private bool TryResolveAssetPath(string relativePath, out string filePath, out string contentType)
    {
        filePath = string.Empty;
        contentType = "application/octet-stream";

        var repoPath = _optionsMonitor.CurrentValue.BlogRepoPath;
        if (string.IsNullOrWhiteSpace(repoPath))
        {
            return false;
        }

        var normalizedRelativePath = relativePath
            .Replace('\\', '/')
            .TrimStart('/');

        if (string.IsNullOrWhiteSpace(normalizedRelativePath) ||
            normalizedRelativePath.Contains("..", StringComparison.Ordinal))
        {
            return false;
        }

        var candidate = Path.GetFullPath(Path.Combine(repoPath, normalizedRelativePath.Replace('/', Path.DirectorySeparatorChar)));
        var root = Path.GetFullPath(repoPath);
        var rootWithSeparator = root.EndsWith(Path.DirectorySeparatorChar) ? root : $"{root}{Path.DirectorySeparatorChar}";

        if (!candidate.StartsWith(rootWithSeparator, StringComparison.Ordinal) || !File.Exists(candidate))
        {
            return false;
        }

        filePath = candidate;
        if (!_contentTypeProvider.TryGetContentType(candidate, out var detectedContentType) || string.IsNullOrWhiteSpace(detectedContentType))
        {
            contentType = "application/octet-stream";
        }
        else
        {
            contentType = detectedContentType;
        }

        return true;
    }
}
