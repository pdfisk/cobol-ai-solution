using System.Data;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Npgsql;

namespace CobolAiDesktop;

public sealed class CobolAiDesktopServer
{
    public const string Url = "http://localhost:5000/";

    private readonly string _assetsRoot;

    private WebApplication? _app;

    // Local DB values from local_db.ps1
    private const string ConnectionString =
        "Host=localhost;Port=5432;Database=cobol_studio;Username=admin;Password=chichi";

    private sealed record ScriptFileDto(string FileName, string Content);

    public CobolAiDesktopServer()
    {
        _assetsRoot = FindCobolAiGuiWwwRoot();
        if (string.IsNullOrWhiteSpace(_assetsRoot))
        {
            throw new DirectoryNotFoundException(
                "Could not locate CobolAiGui published wwwroot. Build CobolAiGui first (Debug) so index.html exists.");
        }
    }

    public async Task StartAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls(Url);

        builder.Services.AddRouting();
        builder.Services.AddCors(options =>
        {
            // Same-origin calls from WebView2 don't require this, but harmless and helps troubleshooting.
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();
        _app = app;

        app.UseCors();
        app.UseDefaultFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(_assetsRoot),
            RequestPath = ""
        });

        // API
        app.MapGet("/cobol", async context =>
        {
            context.Response.ContentType = "application/json";
            var items = await LoadFilesAsync(context, "cobol_source_files");
            await context.Response.WriteAsJsonAsync(items);
        });

        app.MapGet("/python", async context =>
        {
            context.Response.ContentType = "application/json";
            var items = await LoadFilesAsync(context, "python_source_files");
            await context.Response.WriteAsJsonAsync(items);
        });

        // SPA fallback: serve index.html for any other route
        app.MapFallback(async context =>
        {
            context.Response.ContentType = "text/html";
            var indexPath = Path.Combine(_assetsRoot, "index.html");
            if (!File.Exists(indexPath))
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("index.html not found");
                return;
            }

            await context.Response.SendFileAsync(indexPath);
        });

        await app.StartAsync();
    }

    public async Task StopAsync()
    {
        if (_app is null)
        {
            return;
        }

        await _app.StopAsync();
    }

    private static string FindCobolAiGuiWwwRoot()
    {
        // Look upward from the current output directory for CobolAiGui publish assets.
        // Expected dev path:
        //   CobolAiGui/bin/Debug/net10.0/browser-wasm/publish/wwwroot/index.html
        //   CobolAiGui/bin/Release/net10.0/browser-wasm/publish/wwwroot/index.html
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var debugIndex = Path.Combine(
                dir.FullName,
                "CobolAiGui",
                "bin",
                "Debug",
                "net10.0",
                "browser-wasm",
                "publish",
                "wwwroot",
                "index.html");
            if (File.Exists(debugIndex))
            {
                return Path.GetDirectoryName(debugIndex) ?? string.Empty;
            }

            var releaseIndex = Path.Combine(
                dir.FullName,
                "CobolAiGui",
                "bin",
                "Release",
                "net10.0",
                "browser-wasm",
                "publish",
                "wwwroot",
                "index.html");
            if (File.Exists(releaseIndex))
            {
                return Path.GetDirectoryName(releaseIndex) ?? string.Empty;
            }

            dir = dir.Parent;
        }

        return string.Empty;
    }

    private static async Task<List<ScriptFileDto>> LoadFilesAsync(HttpContext context, string tableName)
    {
        // `tableName` is trusted input (hardcoded by our MapGet routes).
        var list = new List<ScriptFileDto>();

        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync(context.RequestAborted);

        await using var cmd = new NpgsqlCommand(
            $"SELECT file_name, content FROM {tableName} ORDER BY file_name",
            conn);

        await using var reader = await cmd.ExecuteReaderAsync(context.RequestAborted);
        while (await reader.ReadAsync(context.RequestAborted))
        {
            list.Add(new ScriptFileDto(
                reader.GetString(0),
                reader.GetString(1)));
        }

        return list;
    }
}

