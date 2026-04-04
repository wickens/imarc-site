using System.Diagnostics;
using System.Text;
using ImarcAdmin.Config;
using Microsoft.Extensions.Options;

namespace ImarcAdmin.Services;

public sealed class GitCommandRunner
{
    private readonly IOptionsMonitor<AdminOptions> _optionsMonitor;
    private readonly ILogger<GitCommandRunner> _logger;

    public GitCommandRunner(IOptionsMonitor<AdminOptions> optionsMonitor, ILogger<GitCommandRunner> logger)
    {
        _optionsMonitor = optionsMonitor;
        _logger = logger;
    }

    public async Task<string> RunAsync(string arguments, string workingDirectory, CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo("git", arguments)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var keyPath = _optionsMonitor.CurrentValue.GitSshKeyPath;
        if (!string.IsNullOrWhiteSpace(keyPath))
        {
            psi.Environment["GIT_SSH_COMMAND"] = $"ssh -i \"{keyPath}\" -o IdentitiesOnly=yes -o StrictHostKeyChecking=accept-new";
        }

        using var process = new Process { StartInfo = psi };
        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        process.OutputDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                stdout.AppendLine(args.Data);
            }
        };

        process.ErrorDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                stderr.AppendLine(args.Data);
            }
        };

        _logger.LogInformation("Running git {Arguments} in {WorkingDirectory}", arguments, workingDirectory);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"git {arguments} failed: {stderr.ToString().Trim()}");
        }

        return stdout.ToString().Trim();
    }
}
