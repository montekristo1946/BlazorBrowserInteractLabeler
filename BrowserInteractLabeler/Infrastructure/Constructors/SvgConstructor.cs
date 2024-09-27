using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Infrastructure.Configs;
using Serilog;

namespace BrowserInteractLabeler.Infrastructure.Constructors;

public class SvgConstructor
{
    private readonly Serilog.ILogger _logger = Log.ForContext<SvgConstructor>();

    private readonly ServiceConfigs _serviceConfigs;

    const string _black = "#000000";
    const string _white = "#ffffff";
    private const double _lineCorrectionFactor = 0.8;
    public SvgConstructor(ServiceConfigs serviceConfigs)
    {
        _serviceConfigs = serviceConfigs ?? throw new ArgumentNullException(nameof(serviceConfigs));
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
                figureRet += CreateSVGPolygon(annotation, activeAnnot, thicknessLine);
                break;
            case TypeLabel.PolyLine:
                figureRet += CreateSVGPolyline(annotation, activeAnnot, thicknessLine);
                break;
            case TypeLabel.Point:
                figureRet += CreateSVGPoint(annotation, activeAnnot, thicknessLine);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        return figureRet;
    }

    private string CreateSVGPolygon(Annotation annotation, bool activeAnnot, double thicknessLine)
    {
        if (annotation.Points?.Any() == false)
            return string.Empty;


        var srcPoints = annotation.Points;

        var retPolygon = new List<string>();
        var colorModel = _serviceConfigs.GetColor(annotation.LabelId);
        var color = colorModel.Color;

        var thickness = _serviceConfigs.StrokeWidth * thicknessLine;
        var typeLine = CrateDottedLine(activeAnnot);

        var drawPoints = new List<(PointF, PointF)>();

        if (activeAnnot && annotation.State == StateAnnot.Active)
        {
            var first = srcPoints.Last();
            var last = srcPoints.First();
            var x1 = first.X;
            var y1 = first.Y;
            var x2 = last.X;
            var y2 = last.Y;

            var lineBlack = CreateLine(x1, y1, x2, y2, _black, thickness, typeLine);
            retPolygon.Add(lineBlack);

            var lineWhite = CreateLine(x1, y1, x2, y2, _white, thickness * _lineCorrectionFactor, typeLine);
            retPolygon.Add(lineWhite);
        }

        for (int i = 0; i < srcPoints.Count - 1; i++)
        {
            var first = srcPoints[i];
            var last = srcPoints[i + 1];
            drawPoints.Add((first, last));
        }

        foreach (var drawPoint in drawPoints)
        {
            var x1 = drawPoint.Item1.X;
            var y1 = drawPoint.Item1.Y;

            var x2 = drawPoint.Item2.X;
            var y2 = drawPoint.Item2.Y;
            var line = CreateLine(x1, y1, x2, y2, color, thickness * _lineCorrectionFactor, typeLine);
            retPolygon.Add(line);
        }

        if (activeAnnot)
        {
            var anchorPoints = CreateAnchorPoints(annotation.Points, thickness, color, thickness);
            retPolygon.AddRange(anchorPoints);
            var lastPoint = CreateLastPoint(annotation.Points, thickness);
            retPolygon.Add(lastPoint);
        }
        else
        {
            var first = srcPoints.Last();
            var last = srcPoints.First();
            var x1 = first.X;
            var y1 = first.Y;

            var x2 = last.X;
            var y2 = last.Y;
            var line = CreateLine(x1, y1, x2, y2, color, thickness * _lineCorrectionFactor, typeLine);
            retPolygon.Add(line);
        }


        return String.Join(" ", retPolygon);
    }

    private string CreateSVGPoint(Annotation annotation, bool activeAnnot, double thicknessLine)
    {
        if (annotation.Points?.Any() == false)
            return string.Empty;

        var srcPoints = annotation.Points;
        var retPolygon = new List<string>();
        var colorModel = _serviceConfigs.GetColor(annotation.LabelId);
        var color = colorModel.Color;
        var thickness = _serviceConfigs.StrokeWidth * thicknessLine;
        var radius = thickness;
        if (activeAnnot)
        {
            var lines = srcPoints
                .Select(p => CreateActivePoint(thickness, p))
                .SelectMany(p => p)
                .ToArray();
            retPolygon.AddRange(lines);
        }
        else
        {
            var anchorPoints = CreateAnchorPoints(srcPoints, radius, color, thickness);
            retPolygon.AddRange(anchorPoints);
        }


        return String.Join(" ", retPolygon);
    }

    private string CreateSVGPolyline(Annotation annotation, bool activeAnnot, double thicknessLine)
    {
        if (annotation.Points?.Any() == false)
            return string.Empty;


        var srcPoints = annotation.Points;

        var retPolygon = new List<string>();
        var colorModel = _serviceConfigs.GetColor(annotation.LabelId);
        var color = colorModel.Color;

        var thickness = _serviceConfigs.StrokeWidth * thicknessLine;
        var typeLine = CrateDottedLine(activeAnnot);


        var drawPoints = new List<(PointF, PointF)>();

        for (var i = 0; i < srcPoints.Count - 1; i++)
        {
            var first = srcPoints[i];
            var last = srcPoints[i + 1];
            drawPoints.Add((first, last));
        }

        foreach (var drawPoint in drawPoints)
        {
            var x1 = drawPoint.Item1.X;
            var y1 = drawPoint.Item1.Y;

            var x2 = drawPoint.Item2.X;
            var y2 = drawPoint.Item2.Y;
            var line = CreateLine(x1, y1, x2, y2, color, thickness * _lineCorrectionFactor, typeLine);


            retPolygon.Add(line);
        }

        if (activeAnnot)
        {
            var anchorPoints = CreateAnchorPoints(annotation.Points, thickness, color, thickness);
            retPolygon.AddRange(anchorPoints);
            var lastPoint = CreateLastPoint(annotation.Points, thickness);
            retPolygon.Add(lastPoint);
        }

        return String.Join(" ", retPolygon);
    }

    private string CreateLastPoint(List<PointF> annotationPoints, double strokeWidth)
    {
        var lastPoint = annotationPoints.MaxBy(p => p.PositionInGroup);
        if (lastPoint is null)
            return string.Empty;

        var retPolygon = CreateActivePoint(strokeWidth, lastPoint);

        return String.Join(" ", retPolygon);
    }

    private static List<string> CreateActivePoint(double strokeWidth, PointF lastPoint)
    {
        var retPolygon = new List<string>();
        var cx = lastPoint.X;
        var cy = lastPoint.Y;

        const double coefBlack = 1;
        // var blackPoint =
        //     $"<circle cx=\"{cx * 100}%\" cy=\"{cy * 100}%\" r=\"{strokeWidth * coefBlack}\" stroke=\"{black}\" stroke-width=\"{strokeWidth }\" fill-opacity=\"1\"></circle>";

        var blackPoint =
            $"<circle cx=\"{cx * 100}%\" cy=\"{cy * 100}%\" r=\"{strokeWidth * coefBlack}\" fill=\"{_black}\" />$";
        const double coefWhite = 0.8;
        // var whitePoint =
        //     $"<circle cx=\"{cx * 100}%\" cy=\"{cy * 100}%\" r=\"{strokeWidth * coefWhite}\" stroke=\"{white}\" stroke-width=\"{10}\" fill-opacity=\"{white}\"></circle>";

        var whitePoint =
            $"<circle cx=\"{cx * 100}%\" cy=\"{cy * 100}%\" r=\"{strokeWidth * coefWhite}\" fill=\"{_white}\" />$";
        retPolygon.Add(blackPoint);
        retPolygon.Add(whitePoint);

        return retPolygon;
    }

    private string CreateSVGBox(Annotation annotation, bool activeAnnot, double thickness)
    {
        if (annotation.Points?.Any() == false)
            return string.Empty;

        var thicknessLine = _serviceConfigs.StrokeWidth * thickness;
        var typeLine = CrateDottedLine(activeAnnot);

        var retSvg = new List<string>();
        var pointSrc = annotation.Points;
        var colorModel = _serviceConfigs.GetColor(annotation.LabelId);
        var color = colorModel.Color;

        // if (pointSrc.Count != 2) 
        //     return String.Join(" ", retSvg);

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

        var line1 = CreateLine(x1, y1, x2, y2, color, thicknessLine * _lineCorrectionFactor, typeLine);
        retSvg.Add(line1);
        var line2 = CreateLine(x2, y2, x3, y3, color, thicknessLine * _lineCorrectionFactor, typeLine);
        retSvg.Add(line2);
        var line3 = CreateLine(x3, y3, x4, y4, color, thicknessLine * _lineCorrectionFactor, typeLine);
        retSvg.Add(line3);
        var line4 = CreateLine(x4, y4, x1, y1, color, thicknessLine * _lineCorrectionFactor, typeLine);
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
        // return
        // $"<circle cx=\"{cx * 100}%\" cy=\"{cy * 100}%\" r=\"{r}\" stroke=\"{color}\" stroke-width=\"{strokeWidth}\" fill=\"{color}\" fill-opacity=\"1\"></circle>";
        return $"<circle cx=\"{cx * 100}%\" cy=\"{cy * 100}%\" r=\"{r}\" fill=\"{color}\" />$";
    }

    private string CreateLine(double x1, double y1, double x2, double y2, string colorModelColor, double strokeWidth,
        string typeLine)
    {
        return
            $"<line x1=\"{x1 * 100}%\" y1=\"{y1 * 100}%\" x2=\"{x2 * 100}%\"  y2=\"{y2 * 100}%\" stroke=\"{colorModelColor}\" stroke-width=\"{strokeWidth}\" {typeLine}> </line>";
    }
}