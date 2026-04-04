namespace ImarcAdmin.Models;

public sealed class PublishResult
{
    public required string PostId { get; init; }
    public required string RelativePath { get; init; }
    public required string HeadCommit { get; init; }
    public required string CommitMessage { get; init; }
    public bool PushedToRemote { get; init; }
}
