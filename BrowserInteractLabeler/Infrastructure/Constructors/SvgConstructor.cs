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

    public string CreateSVG(Annotation[] annotations, double thicknessLine)
    {
        if (annotations?.Any() == false)
            return string.Empty;

        var svg = string.Empty;

      
        foreach (var annotation in annotations)
        {
            if (annotation.Points?.Any() == false)
                continue;

            var activeAnnot = annotation.State != StateAnnot.Finalized;

            switch (annotation.LabelPattern)
            {
                case TypeLabel.None:
                    break;
                case TypeLabel.Box:
                    svg += CreateSVGBox(annotation, activeAnnot, thicknessLine);
                    break;
                case TypeLabel.Polygon:
                    svg += CreateSVGPolygon(annotation, activeAnnot, false, thicknessLine);
                    break;
                case TypeLabel.PolyLine:
                    svg += CreateSVGPolygon(annotation, activeAnnot, true, thicknessLine);
                    break;
                case TypeLabel.Point:
                    svg += CreateSVGPoint(annotation, activeAnnot, thicknessLine);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        return svg;
    }

    public (bool checkCreateSvgCursor, string svgSting ) CreateSvgCursor(PointF point,  string color ="#ffffff")
    {
        if (point is null)
            return (false, string.Empty);
            
        var retBoxs = new List<string>();
        
        var strokeWidth = 1D;
       
        var typeLine = "stroke-dasharray=\"4 2\"";

        const int lenghtLine = 1; //100% width/height img

        var xv1 = point.X;
        var yv1 = point.Y - lenghtLine;
        var xv2 = point.X;
        var yv2 = point.Y + lenghtLine;
        var xh1 = point.X - lenghtLine;
        var yh1 = point.Y;
        var xh2 = point.X + lenghtLine;
        var yh2 = point.Y;

        var line1 = CreateLine(xv1, yv1, xv2, yv2, color, strokeWidth * 0.5f, typeLine);
        retBoxs.Add(line1);
        var line2 = CreateLine(xh1, yh1, xh2, yh2, color, strokeWidth * 0.5f, typeLine);
        retBoxs.Add(line2);

        return (true, String.Join(" ", retBoxs));
    }

    private string CreateSVGPoint(Annotation annotation, bool activeAnnot, double thicknessLine)
    {
        var retPolygon = new List<string>();
        var colorModel = _serviceConfigs.GetColor(annotation.LabelId);
        var color = colorModel.Color;
        var strokeWidth = 2d;

        if (activeAnnot)
            strokeWidth += 1;

        strokeWidth *= thicknessLine;
        var radius = strokeWidth;

        var srcPoints = annotation.Points;
        var anchorPoints = CreateAnchorPoints(srcPoints, radius, color, strokeWidth);
        retPolygon.AddRange(anchorPoints);

        return String.Join(" ", retPolygon);
    }

    private string CreateSVGPolygon(Annotation annotation, bool activeAnnot, bool polyLineType, double thicknessLine)
    {
        const int minDrawPoints = 2;
        var retPolygon = new List<string>();
        var colorModel = _serviceConfigs.GetColor(annotation.LabelId);
        var color = colorModel.Color;

        var strokeWidth = 2D;
        var typeLine = CrateDottedLine(activeAnnot);

        strokeWidth *= thicknessLine;
        var radius = strokeWidth;
        var srcPoints = annotation.Points;


        if (activeAnnot)
        {
            var anchorPoints = CreateAnchorPoints(annotation.Points, radius, color, strokeWidth);
            retPolygon.AddRange(anchorPoints);
        }

        if (srcPoints.Count < minDrawPoints)
            return String.Join(" ", retPolygon);

        var drawPoints = new List<(PointF, PointF)>();

        for (int i = 0; i < srcPoints.Count - 1; i++)
        {
            var first = srcPoints[i];
            var last = srcPoints[i + 1];
            drawPoints.Add((first, last));
        }

        if (!activeAnnot && !polyLineType)
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
        var strokeWidth = 2D;
        var typeLine = CrateDottedLine(activeAnnot);

        // if (activeAnnot)
        //     strokeWidth =2D;

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

        if (annotation.Points.Count == 1)
        {
            const int lenghtLine = 1; //100% width/height img
            var point = annotation.Points.First();
            var xv1 = point.X;
            var yv1 = point.Y - lenghtLine;
            var xv2 = point.X;
            var yv2 = point.Y + lenghtLine;
            var xh1 = point.X - lenghtLine;
            var yh1 = point.Y;
            var xh2 = point.X + lenghtLine;
            var yh2 = point.Y;
        
            var line1 = CreateLine(xv1, yv1, xv2, yv2, color, strokeWidth * 0.5f, typeLine);
            retBoxs.Add(line1);
            var line2 = CreateLine(xh1, yh1, xh2, yh2, color, strokeWidth * 0.5f, typeLine);
            retBoxs.Add(line2);
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
            // var line5 = CreateLine(x1, y1, x3, y3,color, strokeWidth*0.5f,typeLine);
            // retBoxs.Add(line5);
        }

        return String.Join(" ", retBoxs);
    }

    private static string CrateDottedLine(bool activeAnnot)
    {
        var typeLine = "stroke-dasharray=\"4 0\"";
        if (activeAnnot)
        {
            typeLine = "stroke-dasharray=\"4 2\"";
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
}