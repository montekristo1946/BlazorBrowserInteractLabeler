using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BrowserInteractLabeler.Component.DrawingJobBox;

public class SvgPanelModel: ComponentBase
{
    [Parameter] public string SvgToDraw { get; set; }

    internal RenderFragment RenderfragmentToPanel => (builder) =>
    {
        builder.AddMarkupContent(0,$"<svg width=\"100%\" height=\"100%\">{SvgToDraw}</svg>");
    };
   
}