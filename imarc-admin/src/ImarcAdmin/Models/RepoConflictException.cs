namespace ImarcAdmin.Models;

public sealed class RepoConflictException : Exception
{
    public RepoConflictException(string message) : base(message)
    {
    }
}
