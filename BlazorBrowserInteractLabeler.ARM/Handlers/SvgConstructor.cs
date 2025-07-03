using System.Text;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Web.Infrastructure.Buffers;

namespace BlazorBrowserInteractLabeler.ARM.Handlers;

public class SvgConstructor
{
    private readonly SettingsData  _settingsData;
    private readonly AnnotationHandler _annotationHandler;
    private readonly MarkupData _markupData;
    private const double LineCorrectionFactor = 0.8;
    const string _black = "#000000";
    const string _white = "#ffffff";

    public SvgConstructor(SettingsData settingsData, AnnotationHandler annotationHandler, MarkupData markupData)
    {
        _settingsData = settingsData ?? throw new ArgumentNullException(nameof(settingsData));
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
    }

    public async Task<string> CreateAnnotsFigure()
    {

        var annots = await _annotationHandler.GetAllAnnotations();

        if (annots.Length == 0)
           return string.Empty;

        var thicknessLine = _settingsData.StrokeWidth / _markupData.ScaleCurrent; 
        var retStringSvg = new StringBuilder();
        
        foreach (var annotation in annots)
        {
            if(annotation.State == StateAnnot.Hidden)
                continue;
            
            var activeAnnot = annotation.State != StateAnnot.Finalized;
            switch (annotation.LabelPattern)
            {
                case TypeLabel.None:
                    break;
                case TypeLabel.Box:
                    var box = CreateSVGBox(annotation, activeAnnot, thicknessLine);
                    retStringSvg.Append(box);
                    break;
                case TypeLabel.Polygon:
                    break;
                case TypeLabel.PolyLine:
                    break;
                case TypeLabel.Point:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        return retStringSvg.ToString();

    }
    private static string CrateDottedLine(bool activeAnnot)
    {
        var typeLine = "stroke-dasharray=\"4 0\"";
        if (activeAnnot)
        {
            typeLine = "stroke-dasharray=\"4 1\"";
        }

        return typeLine;
    }
    private string CreateSVGBox(Annotation? annotation, bool activeAnnot, double thicknessLine)
    {
        if (annotation?.Points is null || annotation.Points.Any() == false)
            return string.Empty;

   
        var typeLine = CrateDottedLine(activeAnnot);

        var retSvg = new List<string>();
        var pointSrc = annotation.Points;
        var colorModel = _settingsData.GetColor(annotation.LabelId);
        var color = colorModel.Color;

        var points = annotation.Points;
        var leftPoint = points.OrderByDescending(p => p.X).First();
        var rightPoint = points.OrderBy(p => p.X).First();

        var x1 = leftPoint.X;
        var y1 = leftPoint.Y;

        var x2 = rightPoint.X;
        var y2 = leftPoint.Y;

        var x3 = rightPoint.X;
        var y3 = rightPoint.Y;

        var x4 = leftPoint.X;
        var y4 = rightPoint.Y;

        var line1 = CreateLine(x1, y1, x2, y2, color, thicknessLine * LineCorrectionFactor, typeLine);
        retSvg.Add(line1);
        var line2 = CreateLine(x2, y2, x3, y3, color, thicknessLine * LineCorrectionFactor, typeLine);
        retSvg.Add(line2);
        var line3 = CreateLine(x3, y3, x4, y4, color, thicknessLine * LineCorrectionFactor, typeLine);
        retSvg.Add(line3);
        var line4 = CreateLine(x4, y4, x1, y1, color, thicknessLine * LineCorrectionFactor, typeLine);
        retSvg.Add(line4);

        if (activeAnnot)
        {
            var anchorPoints = CreateAnchorPoints(pointSrc, thicknessLine, color, thicknessLine);
            retSvg.AddRange(anchorPoints);
            var lastPoint = CreateLastPoint(pointSrc, thicknessLine);
            retSvg.Add(lastPoint);
        }

        return String.Join(" ", retSvg);
    }
    
    private string[] CreateAnchorPoints(List<PointD> points, double radius, string color, double strokeWidth)
    {
        var retPoints = new List<string>();
        foreach (var annotationPoint in points)
        {
            var cx = annotationPoint.X;
            var cy = annotationPoint.Y;

            var circle = CreateCircle(cx, cy, radius, color);
            retPoints.Add(circle);
        }

        return retPoints.ToArray();
    }
    
    private string CreateCircle(double cx, double cy, double r, string color)
    {
        return $"<circle cx=\"{cx * 100}%\" cy=\"{cy * 100}%\" r=\"{r}\" fill=\"{color}\" />$";
    }
    
    private string CreateLastPoint(List<PointD> annotationPoints, double strokeWidth)
    {
        var lastPoint = annotationPoints.MaxBy(p => p.PositionInGroup);
        if (lastPoint is null)
            return string.Empty;

        var retPolygon = CreateActivePoint(strokeWidth, lastPoint);

        return String.Join(" ", retPolygon);
    }
    private static List<string> CreateActivePoint(double strokeWidth, PointD lastPoint)
    {
        var retPolygon = new List<string>();
        var cx = lastPoint.X;
        var cy = lastPoint.Y;

        const double coefBlack = 1;

        var blackPoint =
            $"<circle cx=\"{cx * 100}%\" cy=\"{cy * 100}%\" r=\"{strokeWidth * coefBlack}\" fill=\"{_black}\" />$";
        const double coefWhite = 0.8;
   
        var whitePoint =
            $"<circle cx=\"{cx * 100}%\" cy=\"{cy * 100}%\" r=\"{strokeWidth * coefWhite}\" fill=\"{_white}\" />$";
        retPolygon.Add(blackPoint);
        retPolygon.Add(whitePoint);

        return retPolygon;
    }
    
    private string CreateLine(
        double x1, 
        double y1, 
        double x2, 
        double y2, 
        string colorModelColor, 
        double strokeWidth,
        string typeLine)
    {
        return
            $"<line x1=\"{x1 * 100}%\" y1=\"{y1 * 100}%\" x2=\"{x2 * 100}%\"  y2=\"{y2 * 100}%\" stroke=\"{colorModelColor}\" stroke-width=\"{strokeWidth}\" {typeLine}> </line>";
    }
}