using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Component.DrawingJobBox;

public class CrosshairModel : ComponentBase
{
    [Parameter] public PointF PointCursor { get; set; }
    [Parameter] public double ScaleCurrent { get; set; }
    [Parameter] public TypeLabel ActiveTypeLabel { get; set; }
    [Parameter] public string ActiveLabelColor { get; set; }
    [Parameter] public Annotation[] AnnotationsOnPanel { get; set; }

    internal string TypeLine = "stroke-dasharray=\"4 2\"";

    internal double GetThicknessLine()
    {
        return 1 / ScaleCurrent;
    }

    private (PointF p1, PointF p2) DefaultPoint()
    {
        return (new PointF() { X = 0, Y = 0 }, new PointF() { X = 0, Y = 0 });
    }

    internal (PointF p1, PointF p2) GetVerticalPoints()
    {
        if (ActiveTypeLabel != TypeLabel.Box)
            return DefaultPoint();

        var xv1 = PointCursor.X * 100;
        var yv1 = 0;
        var xv2 = PointCursor.X * 100;
        var yv2 = 100;

        return (new PointF() { X = xv1, Y = yv1 }, new PointF() { X = xv2, Y = yv2 });
    }

    internal (PointF p1, PointF p2) GetHorizontalPoint()
    {
        if (ActiveTypeLabel != TypeLabel.Box)
            return DefaultPoint();

        var xv1 = 0;
        var yv1 = PointCursor.Y * 100;
        var xv2 = 100;
        var yv2 = PointCursor.Y * 100;

        return (new PointF() { X = xv1, Y = yv1 }, new PointF() { X = xv2, Y = yv2 });
    }
    
    internal (PointF p1, PointF p2) GetLinePoint()
    {
        if (ActiveTypeLabel != TypeLabel.PolyLine && ActiveTypeLabel != TypeLabel.Polygon)
            return DefaultPoint();

        var activeAnnot = AnnotationsOnPanel.FirstOrDefault(p => p.State == StateAnnot.Edit || p.State == StateAnnot.Active);
        if(activeAnnot is null)
            return DefaultPoint();

        var lastPoint = activeAnnot.Points?.MaxBy(p => p.PositionInGroup);
        if(lastPoint is null)
            return DefaultPoint();


        var xv1 = PointCursor.X*100;
        var yv1 = PointCursor.Y*100;
        var xv2 = lastPoint.X*100;
        var yv2 = lastPoint.Y*100;

        return (new PointF() { X = xv1, Y = yv1 }, new PointF() { X = xv2, Y = yv2 });
    }
    
}