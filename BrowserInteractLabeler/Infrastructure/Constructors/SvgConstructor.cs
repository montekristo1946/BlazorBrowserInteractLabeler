using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Infrastructure.Configs;
using Serilog;

namespace BrowserInteractLabeler.Infrastructure.Constructors;

public class SvgConstructor
{
    private readonly Serilog.ILogger _logger = Log.ForContext<SvgConstructor>();

    private readonly ServiceConfigs _serviceConfigs;

    public SvgConstructor(ServiceConfigs serviceConfigs)
    {
        _serviceConfigs = serviceConfigs ?? throw new ArgumentNullException(nameof(serviceConfigs));
    }


    private string CreateSVGPoint(Annotation annotation, bool activeAnnot, double thicknessLine)
    {
        if (annotation.Points?.Any() == false)
            return string.Empty;
        
        var retPolygon = new List<string>();
        var colorModel = _serviceConfigs.GetColor(annotation.LabelId);
        var color = colorModel.Color;
        var strokeWidth = 1.8d;

        if (activeAnnot)
            strokeWidth *= 1.2;

        strokeWidth *= thicknessLine;
        var radius = strokeWidth;

        var srcPoints = annotation.Points;
        var anchorPoints = CreateAnchorPoints(srcPoints, radius, color, strokeWidth);
        retPolygon.AddRange(anchorPoints);

        return String.Join(" ", retPolygon);
    }

    private string CreateSVGPolygon(Annotation annotation, bool activeAnnot, bool polyLineType, double thicknessLine)
    {
        if (annotation.Points?.Any() == false)
            return string.Empty;
        
        const int minDrawPoints = 2;
        var retPolygon = new List<string>();
        var colorModel = _serviceConfigs.GetColor(annotation.LabelId);
        var color = colorModel.Color;

        var strokeWidth = 2d;
        var typeLine = CrateDottedLine(activeAnnot);

        strokeWidth *= thicknessLine;
        var radius = strokeWidth;
        var srcPoints = annotation.Points;

        if (srcPoints.Count < minDrawPoints)
            return String.Join(" ", retPolygon);

        if (activeAnnot)
        {
            var anchorPoints = CreateAnchorPoints(annotation.Points, radius, color, strokeWidth);
            retPolygon.AddRange(anchorPoints);
        }

        var drawPoints = new List<(PointF, PointF)>();

        for (int i = 0; i < srcPoints.Count - 1; i++)
        {
            var first = srcPoints[i];
            var last = srcPoints[i + 1];
            drawPoints.Add((first, last));
        }

        if ((!activeAnnot && !polyLineType) ||
            (annotation.State == StateAnnot.Active && !polyLineType))
        {
            var first = srcPoints.Last();
            var last = srcPoints.First();
            drawPoints.Add((first, last));
        }


        foreach (var drawPoint in drawPoints)
        {
            var x1 = drawPoint.Item1.X;
            var y1 = drawPoint.Item1.Y;

            var x2 = drawPoint.Item2.X;
            var y2 = drawPoint.Item2.Y;
            var line = CreateLine(x1, y1, x2, y2, color, strokeWidth, typeLine);
            retPolygon.Add(line);
        }


        return String.Join(" ", retPolygon);
    }

    private string CreateSVGBox(Annotation annotation, bool activeAnnot, double thicknessLine)
    {
        if (annotation.Points?.Any() == false)
            return string.Empty;
        
        var strokeWidth = 2D;
        var typeLine = CrateDottedLine(activeAnnot);
        
        strokeWidth *= thicknessLine;

        var retBoxs = new List<string>();

        var colorModel = _serviceConfigs.GetColor(annotation.LabelId);
        var color = colorModel.Color;
        if (activeAnnot)
        {
            var radius = strokeWidth;
            var anchorPoints = CreateAnchorPoints(annotation.Points, radius, color, strokeWidth);
            retBoxs.AddRange(anchorPoints);
        }
        

        if (annotation.Points.Count == 2)
        {
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

            var line1 = CreateLine(x1, y1, x2, y2, color, strokeWidth, typeLine);
            retBoxs.Add(line1);
            var line2 = CreateLine(x2, y2, x3, y3, color, strokeWidth, typeLine);
            retBoxs.Add(line2);
            var line3 = CreateLine(x3, y3, x4, y4, color, strokeWidth, typeLine);
            retBoxs.Add(line3);
            var line4 = CreateLine(x4, y4, x1, y1, color, strokeWidth, typeLine);
            retBoxs.Add(line4);
      
        }

        return String.Join(" ", retBoxs);
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

    private string[] CreateAnchorPoints(List<PointF> points, double radius, string color, double strokeWidth)
    {
        var retPoints = new List<string>();
        foreach (var annotationPoint in points)
        {
            var cx = annotationPoint.X;
            var cy = annotationPoint.Y;

            var circle = CreateCircle(cx, cy, radius, color, strokeWidth);
            retPoints.Add(circle);
        }

        return retPoints.ToArray();
    }

    private string CreateCircle(double cx, double cy, double r, string color, double strokeWidth)
    {
        return
            $"<circle cx=\"{cx * 100}%\" cy=\"{cy * 100}%\" r=\"{r}\" stroke=\"{color}\" stroke-width=\"{strokeWidth}\" fill=\"{color}\" fill-opacity=\"1\"></circle>";
    }

    private string CreateLine(double x1, double y1, double x2, double y2, string colorModelColor, double strokeWidth,
        string typeLine)
    {
        return
            $"<line x1=\"{x1 * 100}%\" y1=\"{y1 * 100}%\" x2=\"{x2 * 100}%\"  y2=\"{y2 * 100}%\" stroke=\"{colorModelColor}\" stroke-width=\"{strokeWidth}\"  {typeLine}> </line>";
    }

    public string CreateFigure(Annotation annotation, double thicknessLine)
    {
        var figureRet = string.Empty;


        if (annotation.Points?.Any() == false)
            return figureRet;

        if (annotation.State == StateAnnot.Hidden)
            return figureRet;

        annotation.Points = annotation.Points.OrderBy(p => p.PositionInGroup).ToList();

        var activeAnnot = annotation.State != StateAnnot.Finalized;

        switch (annotation.LabelPattern)
        {
            case TypeLabel.None:
                break;
            case TypeLabel.Box:
                figureRet += CreateSVGBox(annotation, activeAnnot, thicknessLine);
                break;
            case TypeLabel.Polygon:
                figureRet += CreateSVGPolygon(annotation, activeAnnot, false, thicknessLine);
                break;
            case TypeLabel.PolyLine:
                figureRet += CreateSVGPolygon(annotation, activeAnnot, true, thicknessLine);
                break;
            case TypeLabel.Point:
                figureRet += CreateSVGPoint(annotation, activeAnnot, thicknessLine);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        return figureRet;
    }
}