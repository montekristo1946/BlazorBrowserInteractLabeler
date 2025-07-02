using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Web.Component.DrawingJobBox;

public class CrosshairModel : ComponentBase
{
    [Parameter] public PointD PointCursor { get; set; }
    [Parameter] public double ScaleCurrent { get; set; }
    [Parameter] public TypeLabel ActiveTypeLabel { get; set; }
    [Parameter] public string ActiveLabelColor { get; set; }
    [Parameter] public Annotation[] AnnotationsOnPanel { get; set; }

    internal string TypeLine = "stroke-dasharray=\"4 2\"";

    internal double GetThicknessLine()
    {
        return 1 / ScaleCurrent;
    }

    private (PointD p1, PointD p2) DefaultPoint()
    {
        return (new PointD() { X = 0, Y = 0 }, new PointD() { X = 0, Y = 0 });
    }

    private (bool stateEdit, Annotation annot) CheckStateAnnot()
    {
        var activeAnnot = AnnotationsOnPanel.FirstOrDefault(p => p.State == StateAnnot.Edit);
        return activeAnnot is not null ? (true, activeAnnot) : (false, new Annotation());
    }

    internal (PointD p1, PointD p2) GetVerticalPoints()
    {
        if (ActiveTypeLabel != TypeLabel.Box)
            return DefaultPoint();


        if (!CheckStateAnnot().stateEdit)
            return DefaultPoint();

        var xv1 = PointCursor.X * 100;
        var yv1 = 0;
        var xv2 = PointCursor.X * 100;
        var yv2 = 100;

        return (new PointD() { X = xv1, Y = yv1 }, new PointD() { X = xv2, Y = yv2 });
    }

    internal (PointD p1, PointD p2) GetHorizontalPoint()
    {
        if (ActiveTypeLabel != TypeLabel.Box)
            return DefaultPoint();

        if (!CheckStateAnnot().stateEdit)
            return DefaultPoint();

        var activeAnnot = AnnotationsOnPanel.FirstOrDefault(p => p.State == StateAnnot.Edit);
        if (activeAnnot is null)
            return DefaultPoint();

        var xv1 = 0;
        var yv1 = PointCursor.Y * 100;
        var xv2 = 100;
        var yv2 = PointCursor.Y * 100;

        return (new PointD() { X = xv1, Y = yv1 }, new PointD() { X = xv2, Y = yv2 });
    }

    internal (PointD p1, PointD p2) GetLinePoint()
    {
        if (ActiveTypeLabel != TypeLabel.PolyLine && ActiveTypeLabel != TypeLabel.Polygon)
            return DefaultPoint();

        var (state, activeAnnot) = CheckStateAnnot();
        if (!state)
            return DefaultPoint();

        var lastPoint = activeAnnot.Points?.MaxBy(p => p.PositionInGroup);
        if (lastPoint is null)
            return DefaultPoint();


        var xv1 = PointCursor.X * 100;
        var yv1 = PointCursor.Y * 100;
        var xv2 = lastPoint.X * 100;
        var yv2 = lastPoint.Y * 100;

        return (new PointD() { X = xv1, Y = yv1 }, new PointD() { X = xv2, Y = yv2 });
    }

    internal (PointD p1, PointD p2) GetLinePointPolygon()
    {
        if (ActiveTypeLabel != TypeLabel.Polygon)
            return DefaultPoint();

        var (state, activeAnnot) = CheckStateAnnot();
        if (!state)
            return DefaultPoint();

        var firstPoint = activeAnnot.Points?.MinBy(p => p.PositionInGroup);
        if (firstPoint is null)
            return DefaultPoint();

        var xv1 = PointCursor.X * 100;
        var yv1 = PointCursor.Y * 100;
        var xv2 = firstPoint.X * 100;
        var yv2 = firstPoint.Y * 100;

        return (new PointD() { X = xv1, Y = yv1 }, new PointD() { X = xv2, Y = yv2 });
    }
}