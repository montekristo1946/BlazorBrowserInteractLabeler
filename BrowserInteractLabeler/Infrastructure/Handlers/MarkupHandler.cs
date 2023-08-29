using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Infrastructure.Configs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Serilog;

namespace BrowserInteractLabeler.Infrastructure;

public class MarkupHandler
{
    private readonly ServiceConfigs _serviceConfigs;
    private readonly Serilog.ILogger _logger = Log.ForContext<MarkupHandler>();

    internal TypeLabel ActiveTypeLabel { get; set; } = TypeLabel.None;
    public int ActiveIdLabel { get; set; } = -1;

    private (bool isMoved, PointF point, Annotation annot) _movedData = (isMoved: false, point: new PointF(),
        annot: new Annotation());

    // private static readonly TimeSpan _minTimeMove = TimeSpan.FromMilliseconds(1500);
    // private DateTime _timeFirstPoint = DateTime.MinValue;

    public MarkupHandler(ServiceConfigs serviceConfigs)
    {
        _serviceConfigs = serviceConfigs ?? throw new ArgumentNullException(nameof(serviceConfigs));
    }


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
                var resBox = ProcessingBoxAnnotation(pointClick, ActiveIdLabel, annotation);
                return (true, resBox);
            case TypeLabel.Polygon:
            case TypeLabel.PolyLine:
            case TypeLabel.Point:
                var res = ProcessingAddPointAnnotation(pointClick, ActiveIdLabel, annotation, ActiveTypeLabel);
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
        if (lastPoint is null)
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
    /// <param name="sizeDrawImage"></param>
    /// <param name="cacheModelScaleCurrent"></param>
    /// <param name="cacheModelSizeDrawImage"></param>
    /// <param name="now"></param>
    public bool PointSelection(MouseEventArgs mouseEventArgs,
        Annotation annotation,
        DateTime timeClick,
        SizeF sizeDrawImage, double scaleCurrent)
    {
        ResetMovedCache();
        if (annotation.Points?.Any() == false)
            return false;
     
        var currentX = mouseEventArgs.OffsetX / sizeDrawImage.Width;
        var currentY = mouseEventArgs.OffsetY / sizeDrawImage.Height;
    
        var overlapPercentageX = (_serviceConfigs.StrokeWidth / sizeDrawImage.Width) * (1 / scaleCurrent);
        var overlapPercentageY = (_serviceConfigs.StrokeWidth / sizeDrawImage.Height) * (1 / scaleCurrent);

        var movePoint = annotation.Points.FirstOrDefault(p =>
        {
            var deltaX = Math.Abs(p.X - currentX);
            var deltaY = Math.Abs(p.Y - currentY);
            var res = deltaX <= overlapPercentageX && deltaY <= overlapPercentageY;

            return res;
        });
        
        if (movePoint is null)
            return false;

        _movedData = (isMoved: true, point: movePoint, annot: annotation);
        return true;
    }

    public void ResetSelectPoint()
    {
        _movedData = (isMoved: false, point: new PointF(), annot: new Annotation());
    }

    private void ResetMovedCache()
    {
        _movedData = (isMoved: false, point: new PointF(), annot: new Annotation());
    }

    /// <summary>
    ///     Реализация самого перемещения обьекта
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="timeClick"></param>
    public (bool result, Annotation annot) HandlerOnmouseuplAsync(MouseEventArgs mouseEventArgs,
        Annotation annotation,
        DateTime timeClick,
        SizeF sizeImg)
    {
        // if (timeClick - _timeFirstPoint < _minTimeMove)
        //     return (false, new Annotation());
        try
        {
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

            var currentIndex = annot.Points.IndexOf(removePoints);
            var oldPoints = annot.Points[currentIndex];
            var newPoints = oldPoints with
            {
                X = currentX, Y = currentY, Id = 0, PositionInGroup = oldPoints.PositionInGroup
            };
            annot.Points[currentIndex] = newPoints;

            _movedData = (true, newPoints, annot);

            // _timeFirstPoint = timeClick;

            return (true, annot);
        }
        catch (Exception e)
        {
            _logger.Error("[MarkupHandler:HandlerOnmouseuplAsync] {Exception}",e);
        }
        return (false, new Annotation());
    }

    /// <summary>
    ///     Назначит выделленую точку последней в массиве точек
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="annotation"></param>
    /// <param name="sizeDrawImage"></param>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public (bool checkResult, Annotation annotation) RepositioningPoints(Annotation annotation)
    {
        if (annotation.Points?.Any() == false)
            return (false, new Annotation());

        // var checkPointSelection = PointSelection(mouseEventArgs, annotation, dateTime, sizeDrawImage);
        // if (checkPointSelection is false)
        //     return (false, new Annotation());

        var activeAnnot = _movedData.annot;
        var activePoint = _movedData.point;

        switch (activeAnnot.LabelPattern)
        {
            case TypeLabel.None:

            case TypeLabel.Point:
                break;

            case TypeLabel.Polygon:
                return ReshapePositionPointsPolygon(activeAnnot, activePoint);

            case TypeLabel.Box:
            case TypeLabel.PolyLine:
                return ReshapePositionPointsPolyLine(activeAnnot, activePoint);

            default:
                throw new ArgumentOutOfRangeException($"[RepositioningPoints] Fail: {activeAnnot.LabelPattern}");
        }


        return (false, new Annotation());
    }

    private (bool checkResult, Annotation annotation) ReshapePositionPointsPolygon(
        Annotation annotation,
        PointF activePoint)
    {
        var positionActive = activePoint.PositionInGroup;
        var arrIndex = new List<int>();
        var oldPoints = annotation.Points;
        if (oldPoints?.Any() == false)
            return (false, new Annotation());

        var firstGroup = oldPoints.Where(p => p.PositionInGroup > positionActive)
            .Select(p => p.PositionInGroup).ToArray();
        arrIndex.AddRange(firstGroup);
        var lastGroup = oldPoints.Where(p => p.PositionInGroup < positionActive)
            .Select(p => p.PositionInGroup).ToArray();
        arrIndex.AddRange(lastGroup);
        arrIndex.Add(positionActive);

        var sortPoints = arrIndex.Select((oldPosition, index) =>
        {
            var point = oldPoints.First(p => p.PositionInGroup == oldPosition);

            return point with { PositionInGroup = index };
        }).ToList();

        annotation.Points = sortPoints;
        return (true, annotation);
    }

    private (bool checkResult, Annotation annotation) ReshapePositionPointsPolyLine(Annotation annotation,
        PointF activePoint)
    {
        var firstPoint = annotation.Points?.MinBy(p => p.PositionInGroup);
        if (firstPoint is null)
            return (false, new Annotation());

        if (activePoint.PositionInGroup != firstPoint.PositionInGroup)
            return (false, new Annotation());

        var sortPoints = annotation.Points.OrderByDescending(p => p.PositionInGroup).Select((point, index) =>
        {
            point.PositionInGroup = index;
            return point;
        }).ToList();

        annotation.Points = sortPoints;
        return (true, annotation);
    }
}