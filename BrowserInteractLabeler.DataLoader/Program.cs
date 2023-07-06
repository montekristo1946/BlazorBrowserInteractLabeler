using BrowserInteractLabeler.DataLoader;
using Microsoft.Extensions.Hosting;
using Serilog;


await Host.CreateDefaultBuilder(args)
    .Configure(args)
    .UseLogger()
    .RunConsoleAsync();