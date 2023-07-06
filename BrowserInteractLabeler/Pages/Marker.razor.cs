using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Serilog;
using ILogger = Serilog.ILogger;


namespace BrowserInteractLabeler.Pages;

public class MarkerModel : ComponentBase
{
    [Inject] internal NavigationHandler _navigationHandler { get; set; }
    
    [Inject] internal KeyMapHandler _keyMapHandler{ get; set; }
    
    [Inject] internal CacheModel _cacheModel{ get; set; }

    [Inject] internal IJSRuntime _JSRuntime { get; set; }
    
    internal readonly string IdImagesPamel  = "marker_panel";
    internal readonly KeyboardEventArgs _commandGoNext = new() { Key = "f" };
    internal readonly KeyboardEventArgs _commandGoBack = new() { Key = "d" };
    
    
    internal async Task HandleKeyDown(KeyboardEventArgs arg)
    {
        await _keyMapHandler.HandleKeyDownAsync(arg);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_navigationHandler.SetMainFocusRootPanel)
        {
            await _JSRuntime.InvokeVoidAsync("FocusElement", IdImagesPamel);
            await _navigationHandler.CancelFocusRootPanelAsync();
        }
    }

}