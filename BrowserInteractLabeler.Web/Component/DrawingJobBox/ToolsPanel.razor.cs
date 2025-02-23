using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Web.Component.DrawingJobBox;

public class ToolsPanelModel : ComponentBase
{
    [Parameter] public EventCallback ButtonDefaultMoveClick { get; set; }

    [Parameter] public EventCallback ButtonOnRectangleClick { get; set; }

    [Parameter] public EventCallback ButtonOnPolygonClick { get; set; }

    [Parameter] public EventCallback ButtonOnPolyLineClick { get; set; }

    [Parameter] public EventCallback ButtonOnPointsClick { get; set; }

}