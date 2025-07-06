using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Сделает активным Annot
/// </summary>
public class SetEditAnnotBySelectPointQueriesHandler : IRequestHandler<SetEditAnnotBySelectPointQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<SetEditAnnotBySelectPointQueriesHandler>();
    private readonly AnnotationHandler _annotationHandler;

    public SetEditAnnotBySelectPointQueriesHandler(AnnotationHandler annotationHandler)
    {
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
    }

    public async Task<bool> Handle(SetEditAnnotBySelectPointQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null || request.Point == null || request.Point.X <= 0 || request.Point.Y <= 0)
                return false;

            var allAnnots = await _annotationHandler.GetAllAnnotations();
            var currentAnnot = allAnnots.FirstOrDefault(p => p.State == StateAnnot.Edit);
            if (currentAnnot is not null)
                return false;

            var pointClick = request.Point;

            var findAnnotId = allAnnots
                .Where(p => p.State == StateAnnot.Finalized)
            .Select(p =>
            {
                var center = CalculationCenter(p.Points);
                var euclideanDistance = CalculationEuclideanDistance(center, pointClick);
                var checkPerimeter = CheckPerimeter(p.Points, pointClick);
                var idAnnot = p.Id;
                return (idAnnot, checkPerimeter, euclideanDistance);
            })
            .Where(p => p.checkPerimeter)
            .OrderBy(p => p.euclideanDistance)
            .FirstOrDefault().idAnnot;

            var annotIsEdit = allAnnots.FirstOrDefault(p => p.Id == findAnnotId);
            if (annotIsEdit is null)
                return false;

            annotIsEdit.State = StateAnnot.Edit;
            await _annotationHandler.UpdateAllAnnotations(allAnnots);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[InitRectangleAnnotHandler] {@Exception}", e);
        }


        return false;
    }

    private bool CheckPerimeter(List<PointD>? argPoints, PointT pointClick)
    {
        if (argPoints is null || argPoints.Any() is false)
            return false;

        var minX = argPoints.Min(p => p.X);
        var maxX = argPoints.Max(p => p.X);
        var minY = argPoints.Min(p => p.Y);
        var maxY = argPoints.Max(p => p.Y);

        return pointClick.X > minX && pointClick.X < maxX && pointClick.Y > minY && pointClick.Y < maxY;
    }

    private double CalculationEuclideanDistance(PointT firstPoint, PointT secondPoint)
    {
        var deltaX = Math.Abs(firstPoint.X - secondPoint.X);
        var deltaY = Math.Abs(firstPoint.Y - secondPoint.Y);
        var size = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        return size;
    }

    private PointT CalculationCenter(List<PointD>? argPoints)
    {
        if (argPoints is null || argPoints.Any() is false)
            return new PointT();

        var minX = argPoints.Min(p => p.X);
        var maxX = argPoints.Max(p => p.X);
        var minY = argPoints.Min(p => p.Y);
        var maxY = argPoints.Max(p => p.Y);

        return new PointT()
        {
            X = minX + (maxX - minX) / 2,
            Y = minY + (maxY - minY) / 2
        };
    }
}