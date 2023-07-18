using BrowserInteractLabeler.Infrastructure;
using BrowserInteractLabeler.Repository;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Component.DrawingJobBox;

public class ImagesPanelModel : ComponentBase
{
    private string _currentImages;

    [Parameter]
    public string ImagesBase64
    {
        get
        {
            var background = $"url(\"{_currentImages}\")";

            return background;
        }
        set => _currentImages = value;
    }

    [Parameter] public string CssScale { get; set; } = string.Empty; // "transform: scale(1.0) translate(+0%, -0%);";
    [Parameter] public string WidthImg { get; set; } = string.Empty;
    [Parameter] public string HeightImg { get; set; } = string.Empty;
    [Parameter] public string WidthMainWin { get; set; } = string.Empty;
    
    [Parameter] public string HeightMainWin { get; set; } = string.Empty;
  
    [Parameter] public EventCallback<MouseEventArgs> HandleMouse { get; set; }
    
    [Parameter] public EventCallback<MouseEventArgs> HandleRightClick { get; set; }
    
    [Parameter] public EventCallback<MouseEventArgs> HandlerOnmousedown { get; set; }
    
    [Parameter] public EventCallback<MouseEventArgs> HandlerOnmousemove { get; set; }
    
    [Parameter] public RenderFragment SvgPanelTemplate { get; set; }
    
    [Parameter] public string CursorStyle { get; set; } = string.Empty;
    
}