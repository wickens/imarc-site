using ImarcAdmin.Config;
using Microsoft.Extensions.Options;

namespace ImarcAdmin.Middleware;

public sealed class CloudflareAccessMiddleware
{
    private const string HeaderName = "Cf-Access-Authenticated-User-Email";

    private readonly RequestDelegate _next;
    private readonly IOptionsMonitor<AdminOptions> _optionsMonitor;
    private readonly IWebHostEnvironment _environment;

    public CloudflareAccessMiddleware(
        RequestDelegate next,
        IOptionsMonitor<AdminOptions> optionsMonitor,
        IWebHostEnvironment environment)
    {
        _next = next;
        _optionsMonitor = optionsMonitor;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/healthz", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var options = _optionsMonitor.CurrentValue;
        if (_environment.IsDevelopment() && options.AllowLocalBypass)
        {
            await _next(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(options.AdminEmail))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Admin email is not configured.");
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var value))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Cloudflare Access authentication is required.");
            return;
        }

        if (!string.Equals(value.ToString(), options.AdminEmail, StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Authenticated user is not allowed to access this app.");
            return;
        }

        await _next(context);
    }
}
