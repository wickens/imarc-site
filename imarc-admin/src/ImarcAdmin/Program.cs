using ImarcAdmin.Config;
using ImarcAdmin.Middleware;
using ImarcAdmin.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.Configure<AdminOptions>(builder.Configuration.GetSection("Admin"));
builder.Services.AddSingleton<SlugService>();
builder.Services.AddSingleton<FrontMatterService>();
builder.Services.AddSingleton<SimpleMarkdownRenderer>();
builder.Services.AddSingleton<MarkdownPreviewService>();
builder.Services.AddSingleton<PreviewAssetService>();
builder.Services.AddSingleton<GitCommandRunner>();
builder.Services.AddSingleton<TempUploadService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<TempUploadService>());
builder.Services.AddSingleton<GitContentRepository>();

var app = builder.Build();

app.UseMiddleware<CloudflareAccessMiddleware>();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));
app.MapGet("/temp-uploads/{sessionId}/{uploadId}", (string sessionId, string uploadId, TempUploadService tempUploadService) =>
{
    var upload = tempUploadService.FindUpload(sessionId, uploadId);
    if (upload is null || !File.Exists(upload.TempFilePath))
    {
        return Results.NotFound();
    }

    return Results.File(upload.TempFilePath, upload.ContentType);
});
app.MapGet("/wp-content/{**assetPath}", (string assetPath, PreviewAssetService previewAssetService) =>
{
    if (!previewAssetService.TryResolveWpContentAsset(assetPath, out var filePath, out var contentType))
    {
        return Results.NotFound();
    }

    return Results.File(filePath, contentType);
});
app.MapGet("/static/{**assetPath}", (string assetPath, PreviewAssetService previewAssetService) =>
{
    if (!previewAssetService.TryResolveStaticAsset(assetPath, out var filePath, out var contentType))
    {
        return Results.NotFound();
    }

    return Results.File(filePath, contentType);
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
