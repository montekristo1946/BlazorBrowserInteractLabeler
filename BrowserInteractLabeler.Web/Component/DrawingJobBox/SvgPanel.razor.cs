using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Web.Extension;
using BrowserInteractLabeler.Web.Infrastructure.Constructors;
using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Web.Component.DrawingJobBox;

public class SvgPanelModel : ComponentBase
{
    [Parameter] public Annotation[] AnnotationsOnPanel { get; set; }
    [Parameter] public double ScaleImg { get; set; }
    [Parameter] public RenderFragment CrosshairsTemplate { get; set; }
    [Inject] internal SvgConstructor _svgConstructor { get; set; }

    internal RenderFragment GetRenderFragnent(Annotation annotation) => (builder) =>
    {
        var thicknessLine = 1 / ScaleImg;
        var figure = _svgConstructor.CreateFigure(annotation.CloneDeep(), thicknessLine);
        builder.AddMarkupContent(0, figure);
    };

}