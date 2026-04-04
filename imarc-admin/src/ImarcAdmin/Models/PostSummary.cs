namespace ImarcAdmin.Models;

public sealed class PostSummary
{
    public required string PostId { get; init; }
    public required string RelativePath { get; init; }
    public required string Title { get; init; }
    public required string Slug { get; init; }
    public required string Permalink { get; init; }
    public required string Excerpt { get; init; }
    public DateTimeOffset? PublishedAt { get; init; }
}

