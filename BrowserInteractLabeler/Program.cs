using BrowserInteractLabeler;

await Host.CreateDefaultBuilder(args)
    .Configure(args)
    .UseLogger()
    .RunConsoleAsync();