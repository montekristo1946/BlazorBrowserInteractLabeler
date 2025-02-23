using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Web.Component.DrawingJobBox;

public class DrawingPanelModel : ComponentBase, IAsyncDisposable
{
    [Parameter] public RenderFragment ToolsPanelTemplate { get; set; }
    [Parameter] public RenderFragment ImagesPamelTemplate { get; set; }
    [Parameter] public RenderFragment TabBoxPanelTemplate { get; set; }
    
    [Parameter] public EventCallback CancelFocusRootPanel { get; set; }
    
    
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }




}