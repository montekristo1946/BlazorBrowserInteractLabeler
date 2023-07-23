using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Serilog;

namespace BrowserInteractLabeler.Infrastructure;

public class MarkupHandler
{
    private readonly Serilog.ILogger _logger = Log.ForContext<MarkupHandler>();

    internal TypeLabel ActiveTypeLabel { get; set; } = TypeLabel.None;
    public int ActiveIdLabel { get; set; } = -1;

    private (bool isMoved, PointF point, Annotation annot) _movedData = (isMoved: false, point: new PointF(),
        annot: new Annotation());

    private static readonly TimeSpan _minTimeMove = TimeSpan.FromMilliseconds(50);
    private DateTime _timeFirstPoint = DateTime.MinValue;


    public (bool checkRes, Annotation res) HandleMouseClickAsync(MouseEventArgs mouseEventArgs,
        SizeF sizeImg, Annotation annotation)
    {
        if (annotation.State == StateAnnot.Finalized || _movedData.isMoved is true)
            return (false, new Annotation());

        var lastPoint = annotation.Points?.MaxBy(p => p.PositionInGroup);
        var lastIPosition = lastPoint == null ? 0 : lastPoint.PositionInGroup + 1;
        
        var pointClick = new PointF()
        {
            X = (float)mouseEventArgs.OffsetX / sizeImg.Width,
            Y = (float)mouseEventArgs.OffsetY / sizeImg.Height,
            Annot = new Annotation(),
            PositionInGroup = lastIPosition
        };

        switch (ActiveTypeLabel)
        {
            case TypeLabel.None:
                break;
            case TypeLabel.Box:
                var resBox =  ProcessingBoxAnnotation(pointClick, ActiveIdLabel, annotation);
                return (true, resBox);
            case TypeLabel.Polygon:
            case TypeLabel.PolyLine:
            case TypeLabel.Point:
                var res =  ProcessingAddPointAnnotation(pointClick, ActiveIdLabel, annotation, ActiveTypeLabel);
                return (true, res);

            default:
                throw new ArgumentOutOfRangeException();
        }

        return (false, new Annotation());
    }

    /// <summary>
    ///     Удалить последнюю точку
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="annot"></param>
    /// <returns></returns>
    public (bool res, Annotation annotation) HandleMouseClickUndoPointAsync(Annotation annot)
    {

        if (annot.State == StateAnnot.Finalized || annot.Points?.Any() == false)
            return (false, annot);

        var lastPoint = annot.Points?.MaxBy(p => p.PositionInGroup);
        if(lastPoint is null)
            return (false, annot);

        annot.Points.Remove(lastPoint);
        
        return (true, annot);
    }


    private Annotation ProcessingAddPointAnnotation(PointF pointClick,
        int activeIdLabel,
        Annotation annotation,
        TypeLabel labelPattern)
    {
        annotation.Points.Add(pointClick);
        annotation.LabelPattern = labelPattern;
        annotation.LabelId = activeIdLabel;

        return annotation;
    }


    private Annotation ProcessingBoxAnnotation(PointF point, int activeIdLabel, Annotation annotation)
    {
        const int maxPoints = 2;
        if (annotation.Points.Count < maxPoints)
        {
            annotation.Points.Add(point);
            annotation.LabelPattern = ActiveTypeLabel;
            annotation.LabelId = activeIdLabel;
        }

        if (annotation.Points.Count == maxPoints)
        {
            annotation.Points.Remove(annotation.Points.Last());
            annotation.Points.Add(point);
            annotation.LabelPattern = ActiveTypeLabel;
            annotation.LabelId = activeIdLabel;
        }

        return annotation;
    }

    


    /// <summary>
    ///     Первое нажатие пред перемещением
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="annotation"></param>
    /// <param name="timeClick"></param>
    /// <param name="cacheModelSizeDrawImage"></param>
    /// <param name="now"></param>
    public void HandlerOnmousedownAsync(MouseEventArgs mouseEventArgs,
        Annotation annotation,
        DateTime timeClick,
        SizeF sizeDrawImage)
    {
        ResetMovedCache();
        var currentX = mouseEventArgs.OffsetX / sizeDrawImage.Width;
        var currentY = mouseEventArgs.OffsetY / sizeDrawImage.Height;
        const int maxPixelOverlapHaloPx = 5;
        var overlapPercentageX = maxPixelOverlapHaloPx / sizeDrawImage.Width;
        var overlapPercentageY = maxPixelOverlapHaloPx / sizeDrawImage.Height;
        var movePoint = annotation.Points.FirstOrDefault(p => Math.Abs(p.X - currentX) < overlapPercentageX &&
                                                              Math.Abs(p.Y - currentY) < overlapPercentageY);
        if (movePoint is null)
            return;

        _timeFirstPoint = timeClick;
        _movedData = (isMoved: true, point: movePoint, annot: annotation);
    }
    
    public void ResetSelectPoint()
    {
        _movedData = (isMoved: false, point: new PointF(), annot: new Annotation());
    }

    private void ResetMovedCache()
    {
        _movedData = (isMoved: false, point: new PointF(), annot: new Annotation());
        _timeFirstPoint = DateTime.MinValue;
    }

    /// <summary>
    ///     Реализация самого перемещения обьекта
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="timeClick"></param>
    public  (bool result, Annotation annot) HandlerOnmouseuplAsync(MouseEventArgs mouseEventArgs,
        Annotation annotation,
        DateTime timeClick,
        SizeF sizeImg)
    {
        if (timeClick - _timeFirstPoint < _minTimeMove)
            return (false, new Annotation());

        if (!_movedData.isMoved || mouseEventArgs.Buttons != 1 || annotation.Id != _movedData.annot.Id)
        {
            ResetMovedCache();
            return (false, new Annotation());
        }


        if (mouseEventArgs.OffsetX > sizeImg.Width ||
            mouseEventArgs.OffsetY > sizeImg.Height ||
            mouseEventArgs.OffsetX < 0 ||
            mouseEventArgs.OffsetY < 0)
        {
            return (false, new Annotation());
        }

        var currentX = (float)(mouseEventArgs.OffsetX / sizeImg.Width);
        var currentY = (float)(mouseEventArgs.OffsetY / sizeImg.Height);
        var movePoint = _movedData.point;
        var annot = _movedData.annot;
        var removePoints = annot.Points.FirstOrDefault(p => p == movePoint);
        if (removePoints is null)
            return (false, new Annotation());

        const int maxPixelOverlapHaloPx = 1;
        const float edgeImg = 1.0F;
        var minPercentStepX = maxPixelOverlapHaloPx / sizeImg.Width;
        var minPercentStepY = maxPixelOverlapHaloPx / sizeImg.Height;

        if ((Math.Abs(currentX - removePoints.X) < minPercentStepX &&
             Math.Abs(currentY - removePoints.Y) < minPercentStepY) ||
            currentX > edgeImg || currentY > edgeImg ||
            currentX < 0 || currentY < 0)
            return (false, new Annotation());

        // _logger.Debug($"[test] {currentX} {currentY}");
        var currentIndex = annot.Points.IndexOf(removePoints);
        var oldPoints = annot.Points[currentIndex];
        var newPoints = oldPoints with { X = currentX, Y = currentY, Id = 0 , PositionInGroup = oldPoints.PositionInGroup};
        // var newPoints = new PointF() { X = currentX, Y = currentY, AnnotationId = oldPoints.AnnotationId, Id = 0};
        annot.Points[currentIndex] = newPoints;

        _movedData = (true, newPoints, annot);

        _timeFirstPoint = timeClick;

        return (true, annot);
    }


  
}