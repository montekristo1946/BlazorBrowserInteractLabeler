using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Web.Component.DrawingJobBox;

public class ToolsPanelModel : ComponentBase
{
    [Parameter] public Action ButtonDefaultMoveClick { get; set; }
    [Parameter] public Action ButtonOnRectangleClick { get; set; }

    [Parameter] public Action ButtonOnPolygonClick { get; set; }

    [Parameter] public Action ButtonOnPolyLineClick { get; set; }

    [Parameter] public Action ButtonOnPointsClick { get; set; }
    
    
    internal void ButtonDefaultMoveOnClick()
    {
        ButtonDefaultMoveClick?.Invoke();
    }

    internal void ButtonOnRectangleOnClick()
    {
        ButtonOnRectangleClick?.Invoke();
    }

    internal void ButtonOnPolygonOnClick()
    {
        ButtonOnPolygonClick?.Invoke();
    }

    internal void ButtonOnPolyLineOnClick()
    {
        ButtonOnPolyLineClick?.Invoke();
    }

    internal void ButtonOnPointsOnClick()
    {
        ButtonOnPointsClick?.Invoke();
    }
    

}