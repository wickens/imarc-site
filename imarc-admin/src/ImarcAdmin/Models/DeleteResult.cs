namespace ImarcAdmin.Models;

public sealed class DeleteResult
{
    public required string RelativePath { get; init; }
    public required string HeadCommit { get; init; }
    public required string CommitMessage { get; init; }
    public bool PushedToRemote { get; init; }
}
