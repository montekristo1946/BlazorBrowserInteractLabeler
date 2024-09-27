using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Shared;

public class NavMenuModel : ComponentBase
{
    private readonly ILogger _logger = Log.ForContext<NavMenuModel>();
    [Inject] public IJSRuntime JSRuntime { get; set; }
    [Inject] internal NavigationHandler _navigationHandler { get; set; }
    [Inject] internal ProjectsLocalHandler _projectsLocalHandler { get; set; }
    [Inject] internal SettingsHandler _settingsHandler { get; set; }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {

        var sizeBrowse = await JSRuntime.InvokeAsync<SizeF>("GetBrowseSize");
        _navigationHandler.SetRootWindowsSize(sizeBrowse);
        _projectsLocalHandler.SetRootWindowsSize(sizeBrowse);
        _settingsHandler.SetRootWindowsSize(sizeBrowse);


    }

}