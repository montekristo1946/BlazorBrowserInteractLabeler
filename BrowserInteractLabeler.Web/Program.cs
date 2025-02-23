
using BrowserInteractLabeler.Web.Extension;

await Host.CreateDefaultBuilder(args)
    .Configure(args)
    .UseLogger()
    .RunConsoleAsync();