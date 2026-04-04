using ImarcAdmin.Config;
using ImarcAdmin.Models;
using Microsoft.Extensions.Options;

namespace ImarcAdmin.Services;

public sealed class MarkdownPreviewService
{
    private const string FallbackCss = """
        .content { max-width: 760px; margin: 0 auto; font-family: "Fira Sans", system-ui, sans-serif; line-height: 1.6; }
        .content img { max-width: 100%; height: auto; border-radius: 12px; }
        .content a { color: #d0007f; }
        .content code { background: #f4f4f4; padding: 0.1rem 0.35rem; border-radius: 6px; }
        .content pre code { display: block; padding: 1rem; overflow-x: auto; }
        .meta { color: #666; font-size: 0.9rem; margin-bottom: 1rem; }
        blockquote { border-left: 3px solid #d0007f; margin: 1rem 0; padding-left: 1rem; color: #444; }
        """;

    private readonly IOptionsMonitor<AdminOptions> _optionsMonitor;
    private readonly SimpleMarkdownRenderer _renderer;

    public MarkdownPreviewService(IOptionsMonitor<AdminOptions> optionsMonitor, SimpleMarkdownRenderer renderer)
    {
        _optionsMonitor = optionsMonitor;
        _renderer = renderer;
    }

    public string Render(EditablePost post, IReadOnlyList<UploadedImageDescriptor> uploads)
    {
        var body = post.MarkdownBody;
        foreach (var upload in uploads)
        {
            body = body.Replace(upload.PublicUrl, upload.PreviewUrl, StringComparison.Ordinal);
        }

        var css = TryReadBlogCss();
        var html = _renderer.Render(body);

        return $"""
            <style>{css}</style>
            <article class="content">
              <h1>{System.Net.WebUtility.HtmlEncode(post.Title)}</h1>
              <p class="meta">{FormatDate(post.PublishDate)}</p>
              {html}
            </article>
            """;
    }

    private string TryReadBlogCss()
    {
        var repoPath = _optionsMonitor.CurrentValue.BlogRepoPath;
        if (string.IsNullOrWhiteSpace(repoPath))
        {
            return FallbackCss;
        }

        var path = Path.Combine(repoPath, "static", "css", "site.css");
        return File.Exists(path) ? File.ReadAllText(path) : FallbackCss;
    }

    private static string FormatDate(string date)
        => DateTime.TryParse(date, out var parsed) ? parsed.ToString("dd MMM yyyy") : date;
}

