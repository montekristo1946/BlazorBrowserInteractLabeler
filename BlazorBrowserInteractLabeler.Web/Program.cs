using BlazorBrowserInteractLabeler.ARM.Handlers;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BlazorBrowserInteractLabeler.Web.AppWeb;
using BlazorBrowserInteractLabeler.Web.Config;
using BlazorBrowserInteractLabeler.Web.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(SystemConfigure.AppSetting["UseUrls"] ?? string.Empty);

builder.Host.UseLogger();
builder.Host.InitCulture();

builder.Services
    .AddSingleton<MarkupData>(MarkupDataBuilder.Build)
    .AddScoped<KeyMapHandler>()
    .AddScoped<Helper>()
    .AddScoped<MoveImagesHandler>()
    .AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

Log.Information("Camera thermal server  started url {UseUrls}", SystemConfigure.AppSetting["UseUrls"]);

app.Run();


