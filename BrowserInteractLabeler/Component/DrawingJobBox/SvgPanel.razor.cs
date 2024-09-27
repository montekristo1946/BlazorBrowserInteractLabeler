using System.Reflection.Metadata;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Infrastructure.Constructors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Serilog;

namespace BrowserInteractLabeler.Component.DrawingJobBox;

public class SvgPanelModel : ComponentBase
{
    [Parameter] public Annotation[] AnnotationsOnPanel { get; set; }
    [Parameter] public double ScaleImg { get; set; }
    [Parameter] public RenderFragment CrosshairsTemplate { get; set; }
    [Inject] internal SvgConstructor _svgConstructor { get; set; }

    internal RenderFragment GetRenderFragnent(Annotation annotation) => (builder) =>
    {
        var thicknessLine = 1 / ScaleImg;
        var figure = _svgConstructor.CreateFigure(annotation, thicknessLine);
        builder.AddMarkupContent(0, figure);
    };

}