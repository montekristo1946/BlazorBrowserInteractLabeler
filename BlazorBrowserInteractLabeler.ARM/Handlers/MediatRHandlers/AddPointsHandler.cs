using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Добавить точку в Annot.
/// </summary>
public class AddPointsHandler : IRequestHandler<AddPointsQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<AddPointsHandler>();
    private readonly AnnotationHandler _annotationHandler;

    public AddPointsHandler( AnnotationHandler annotationHandler)
    {
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
    }

    public async Task<bool> Handle(AddPointsQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;
            
            var allAnnots = await _annotationHandler.GetAllAnnotations();
            var currentAnnot = allAnnots.FirstOrDefault(p => p.State == StateAnnot.Edit);
            if (currentAnnot is null)
                return false;

            
            var tabelPattern = currentAnnot.LabelPattern;
            var point = request.Point;
            currentAnnot = AddPoint(point, tabelPattern, currentAnnot);

            var otherAnnots = allAnnots.Where(p => p.State != StateAnnot.Edit);
            var saveAnnots = otherAnnots.Append(currentAnnot).ToArray();

            await _annotationHandler.UpdateAllAnnotations(saveAnnots);
            
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[InitRectangleAnnotHandler] {@Exception}", e);
        }


        return false;
    }

    private Annotation AddPoint(PointT point, TypeLabel tabelPattern, Annotation annotation)
    {
        return tabelPattern switch
        {
            TypeLabel.None => throw new ArgumentOutOfRangeException(nameof(tabelPattern), tabelPattern, null),
            TypeLabel.Box => AddInBox(annotation, point),
            TypeLabel.Polygon => AddInPolygon(annotation, point),
            TypeLabel.PolyLine => AddInPolyline(annotation, point),
            TypeLabel.Point => AddInPoint(annotation, point),
            _ => throw new ArgumentOutOfRangeException(nameof(tabelPattern), tabelPattern, null)
        };
    }

    private Annotation AddInPoint(Annotation annotation, PointT point)
    {
        return AddInPolygon(annotation, point);
    }

    private Annotation AddInPolyline(Annotation annotation, PointT point)
    {
       return AddInPolygon(annotation, point);
    }

    private Annotation AddInPolygon(Annotation annotation, PointT point)
    {
        annotation.Points ??= new List<PointD>();
        
        var lastPosition = annotation.Points?.MaxBy(p=>p.PositionInGroup)?.PositionInGroup ?? 0;

        annotation.Points?.Add(new PointD()
        {
            X = point.X,
            Y = point.Y,
            PositionInGroup = lastPosition+1
        });
        return annotation;
    }

    private Annotation AddInBox(Annotation annotation, PointT point)
    {
        const int maxPoints = 2;
        annotation.Points ??= new List<PointD>();

        if (annotation.Points.Count == maxPoints)
        {
            annotation.Points.Remove(annotation.Points.Last());
        }

        var lastPosition = annotation.Points?.MaxBy(p=>p.PositionInGroup)?.PositionInGroup ?? 0;

        annotation.Points?.Add(new PointD()
        {
            X = point.X,
            Y = point.Y,
            PositionInGroup = lastPosition+1
        });
        return annotation;
    }
}