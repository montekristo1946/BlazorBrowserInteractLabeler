using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Serilog;

namespace BrowserInteractLabeler.Component.DrawingJobBox;

public class DrawingPanelModel : ComponentBase,IAsyncDisposable
{
    [Parameter] public RenderFragment ToolsPanelTemplate { get; set; }
    [Parameter] public RenderFragment ImagesPamelTemplate { get; set; }
    [Parameter] public RenderFragment TabBoxPanelTemplate { get; set; }
    
    [Parameter] public bool SetMainFocusRootPanel { get; set; }
    
    [Parameter] public EventCallback CancelFocusRootPanel { get; set; }
    
    [Parameter] public EventCallback<WheelEventArgs> HandleMouseWheel { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> HandlerOnmousemove { get; set; }


    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
    



}