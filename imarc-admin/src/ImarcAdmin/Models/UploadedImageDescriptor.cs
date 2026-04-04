namespace ImarcAdmin.Models;

public sealed class UploadedImageDescriptor
{
    public required string Id { get; init; }
    public required string SessionId { get; init; }
    public required string OriginalFileName { get; init; }
    public required string RelativeUploadPath { get; init; }
    public required string PublicUrl { get; init; }
    public required string MarkdownSnippet { get; init; }
    public required string PreviewUrl { get; init; }
    public required string TempFilePath { get; init; }
    public required string ContentType { get; init; }
    public DateTimeOffset UploadedAtUtc { get; init; }
}

