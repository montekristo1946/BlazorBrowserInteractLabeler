using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Перемещение точки в Annot.
/// </summary>
public class MovingPointHandler : IRequestHandler<MovingPointQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<MovingPointHandler>();
    private readonly AnnotationHandler _annotationHandler;
    private readonly MovingPointData _movingPointData;

    public MovingPointHandler(AnnotationHandler annotationHandler, MovingPointData movingPointData)
    {
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
        _movingPointData = movingPointData ?? throw new ArgumentNullException(nameof(movingPointData));
    }

    public async Task<bool> Handle(MovingPointQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null || request.Point.X <= 0 || request.Point.Y <= 0)
                return false;


            var allAnnots = await _annotationHandler.GetAllAnnotations();
            var currentAnnot = allAnnots.FirstOrDefault(p => p.Id == _movingPointData.CurrentIdAnnot && p.State == StateAnnot.Edit);
            if (currentAnnot is null)
                return false;

            var points = currentAnnot.Points;
            if (points is null || !points.Any())
                return false;

            var pointRemove = points.FirstOrDefault(p => p.PositionInGroup == _movingPointData.PositionInGroup);
            if (pointRemove is null)
                return false;

            points = points.Where(p => p.PositionInGroup != _movingPointData.PositionInGroup).ToList();
            var newPoint = request.Point;
            points.Add(new PointD()
            {
                X = newPoint.X,
                Y = newPoint.Y,
                PositionInGroup = pointRemove.PositionInGroup,
                AnnotationId = pointRemove.AnnotationId
            });

            currentAnnot.Points = points;

            await _annotationHandler.UpdateAllAnnotations(allAnnots);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[MovingPointsHandler] {@Exception}", e);
        }


        return false;
    }


}