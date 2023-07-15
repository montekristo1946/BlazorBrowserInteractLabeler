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


    public async Task<(bool checkRes, Annotation res)> HandleMouseClickAsync(MouseEventArgs mouseEventArgs,
        SizeF sizeImg, Annotation annotation)
    {
        if (annotation.State != StateAnnot.Edit)
            return (false, new Annotation());

        var pointClick = new PointF()
        {
            X = (float)mouseEventArgs.OffsetX / sizeImg.Width,
            Y = (float)mouseEventArgs.OffsetY / sizeImg.Height,
            Annot = new Annotation()
        };

        switch (ActiveTypeLabel)
        {
            case TypeLabel.None:
                break;
            case TypeLabel.Box:
                var resBox = await ProcessingBoxAnnotation(pointClick, ActiveIdLabel, annotation);
                return (true, resBox);
            case TypeLabel.Polygon:
            case TypeLabel.PolyLine:
            case TypeLabel.Point:
                var res = await ProcessingAddPointAnnotation(pointClick, ActiveIdLabel, annotation, ActiveTypeLabel);
                return (true, res);

            default:
                throw new ArgumentOutOfRangeException();
        }

        return (false, new Annotation());
    }


    private async Task<Annotation> ProcessingAddPointAnnotation(PointF pointClick,
        int activeIdLabel,
        Annotation annotation,
        TypeLabel labelPattern)
    {
        annotation.Points.Add(pointClick);
        annotation.LabelPattern = labelPattern;
        annotation.LabelId = activeIdLabel;

        return annotation;
    }


    private async Task<Annotation> ProcessingBoxAnnotation(PointF point, int activeIdLabel, Annotation annotation)
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

    /*   private async Task ProcessingPolygonAnnotation(PointF point, int labelId)
      {
          var annotation = await _navigationHandler.CreateNewAnnotation(_idAnnotation);
          _idAnnotation = annotation.Id;
          annotation.Points.Add(point);
          annotation.LabelPattern = _currentTypeLabel;
          annotation.LabelId = labelId;
          
          await _navigationHandler.UpdateAnnotation(annotation);
          await _navigationHandler.SetActiveIdAnnotation(annotation.Id );
      }
  
  
      public async Task HandlerSetLabelIdAsync(int labelId)
      {
          _labelId = labelId;
          var activeIdAnnotation = await _navigationHandler.GerActiveIdAnnotation();
          var allAnnot = await _navigationHandler.GetAnnotationsOnPanel();
          var annotation = allAnnot.FirstOrDefault(p => p.Id == activeIdAnnotation);
          if (annotation is null)
              return;
  
          annotation.LabelId = _labelId;
          await _navigationHandler.UpdateAnnotation(annotation);
          await _navigationHandler.UpdateSvg();
      }*/

    // public async Task StartMarkupAnnot()
    // {
    //     if (_currentTypeLabel == TypeLabel.None || _labelId < 0)
    //     {
    //         await _navigationHandler.SetActiveIdAnnotation(-1);
    //         return;
    //     }
    //
    //     _startMarkupAnnot = !_startMarkupAnnot;
    //
    //     if (_startMarkupAnnot is false)
    //     {
    //         await _navigationHandler.SetActiveIdAnnotation(-1);
    //         await DefaultState();
    //     }
    // }


    /// <summary>
    ///     Первое нажатие пред перемещением
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="annotation"></param>
    /// <param name="timeClick"></param>
    /// <param name="cacheModelSizeDrawImage"></param>
    /// <param name="now"></param>
    public async Task HandlerOnmousedownAsync(MouseEventArgs mouseEventArgs,
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

    private void ResetMovedCache()
    {
        _movedData = (isMoved: false, point: new PointF(), annot: new Annotation());
        _timeFirstPoint = DateTime.MinValue;
    }

    /// <summary>
    /// Реализация самого перемещения обьекта
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="timeClick"></param>
    public async Task<(bool result, Annotation annot)> HandlerOnmouseuplAsync(MouseEventArgs mouseEventArgs,
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
        var newPoints = new PointF() { X = currentX, Y = currentY, AnnotationId = oldPoints.AnnotationId, Id = 0};
        annot.Points[currentIndex] = newPoints;


        _movedData = (true, newPoints, annot);

        _timeFirstPoint = timeClick;

        return (true, annot);
    }

    // public async Task DefaultState()
    // {
    //     _idAnnotation = -1;
    //     // _startMarkupAnnot = false;
    // }
}