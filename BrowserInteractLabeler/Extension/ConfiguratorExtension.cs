using System.Globalization;
using BrowserInteractLabeler;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.RollingFileSizeLimit.Extensions;
using Serilog.Sinks.RollingFileSizeLimit.Impl;

namespace BrowserInteractLabeler;

public static class ConfiguratorExtension
{
    public static IHostBuilder Configure(this IHostBuilder builder, params string[] args) => builder
        .ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddCommandLine(args);
            configurationBuilder.AddEnvironmentVariables();
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", true);

        }).ConfigureWebHostDefaults(webBuilder =>
        {
            var urls = "http://localhost:5001";
            webBuilder.UseStartup<Startup>();
            webBuilder.UseUrls(urls);
            webBuilder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine($"start: {urls}");

        });
    
}