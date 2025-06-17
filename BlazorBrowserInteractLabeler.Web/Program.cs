
using BlazorBrowserInteractLabeler.Web.AppWeb;
using BlazorBrowserInteractLabeler.Web.Config;
using BlazorBrowserInteractLabeler.Web.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(SystemConfigure.AppSetting["UseUrls"] ?? string.Empty);

builder.Host.UseLogger();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();



var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

Log.Information("Camera thermal server  started url {UseUrls}", SystemConfigure.AppSetting["UseUrls"]);

app.Run();