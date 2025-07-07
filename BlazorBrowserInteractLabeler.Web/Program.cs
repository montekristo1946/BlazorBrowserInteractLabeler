using BlazorBrowserInteractLabeler.ARM.Extension;
using BlazorBrowserInteractLabeler.ARM.Handlers;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BlazorBrowserInteractLabeler.Web.AppWeb;
using BlazorBrowserInteractLabeler.Web.Config;
using BlazorBrowserInteractLabeler.Web.Extensions;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(SystemConfigure.AppSetting["UseUrls"] ?? string.Empty);

builder.Host.UseLogger();
builder.Host.InitCulture();
builder.Host.UseMediatRComponents();

builder.Services
    .AddSingleton<MarkupData>(MarkupDataBuilder.Build)
    .AddSingleton<MovingPointData>()
    .AddSingleton<SettingsData>()
    .AddSingleton<IRepository>(provider => new SqlRepository())
    .AddSingleton<ProjectsLocalHandler>(ProjectsLocalHandlerBuilder.Build)
    .AddSingleton<AnnotationHandler>()
    .AddScoped<Mappers>()
    .AddScoped<KeyMapHandler>()
    .AddScoped<Helper>()
    .AddScoped<MoveImagesHandler>()
    .AddScoped<SvgConstructor>()
    
    .AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

Log.Information("Camera thermal server  started url {UseUrls}", SystemConfigure.AppSetting["UseUrls"]);

app.Run();


