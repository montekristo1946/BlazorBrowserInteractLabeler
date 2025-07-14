using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.DataLoader.Tools;
using BrowserInteractLabeler.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.RollingFileSizeLimit.Extensions;
using Serilog.Sinks.RollingFileSizeLimit.Impl;

namespace BrowserInteractLabeler.DataLoader;

public static class ConfiguratorExtension
{
    public static IHostBuilder Configure(this IHostBuilder builder, params string[] args) => builder
        .ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddCommandLine(args);
            configurationBuilder.AddEnvironmentVariables();
        })
        .ConfigureServices((context, services) =>
        {

            var configuration = context.Configuration;
            var pathImg = configuration["pathImg"];
            if (string.IsNullOrEmpty(pathImg))
            {
                Helper.HelperPrint();
                throw new Exception("Fail argument pathImg");
            }

            var typeWork = configuration["typeWork"];
            if (string.IsNullOrEmpty(typeWork))
            {
                Helper.HelperPrint();
                throw new Exception("Fail argument typeWork");
            }

            services.AddScoped<IRepository, SqlRepository>();
            services.AddHostedService<CreateImageDataset>(provider => new CreateImageDataset(pathImg, typeWork));

        });
}