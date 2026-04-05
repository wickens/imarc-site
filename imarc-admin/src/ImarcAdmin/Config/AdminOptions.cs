namespace ImarcAdmin.Config;

public sealed class AdminOptions
{
    public string BlogRepoPath { get; set; } = string.Empty;
    public string TempPath { get; set; } = string.Empty;
    public string GitAuthorName { get; set; } = "iMarc Admin";
    public string GitAuthorEmail { get; set; } = "admin@imarc.co.uk";
    public string GitSshKeyPath { get; set; } = string.Empty;
    public string BaseSiteUrl { get; set; } = "https://www.imarc.co.uk";
    public string BlogRepoRemote { get; set; } = "git@github.com:wickens/imarc-site.git";
    public bool PushChangesToRemote { get; set; } = true;
    public int PostListCacheSeconds { get; set; } = 30;
    public long MaxUploadSizeBytes { get; set; } = 15 * 1024 * 1024;
    public int TempSessionLifetimeHours { get; set; } = 24;
    public int TempCleanupIntervalMinutes { get; set; } = 30;
}
