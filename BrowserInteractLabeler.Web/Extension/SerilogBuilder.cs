using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.RollingFileSizeLimit.Extensions;
using Serilog.Sinks.RollingFileSizeLimit.Impl;

namespace BrowserInteractLabeler.Web.Extension;

public static class SerilogBuilder
{
    public static IHostBuilder UseLogger(this IHostBuilder builder) => builder
        .UseSerilog((ctx, logCfg) =>
        {
            var logDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            var outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";
            logCfg
                .Enrich.WithProcessName()
                .Enrich.WithProcessId()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .MinimumLevel.Is(LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.RollingFileSizeLimited(logDirectoryPath,
                    Path.Combine(logDirectoryPath, "Archive"),
                    fileSizeLimitBytes: 50 * 1024 * 1024,
                    archiveSizeLimitBytes: 10 * 50 * 1024 * 1024,
                    logFilePrefix: nameof(BrowserInteractLabeler),
                    fileCompressor: new DefaultFileCompressor(),
                    outputTemplate: outputTemplate)
                .WriteTo.Console(outputTemplate: outputTemplate);
        });
}