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
public class DeleteLastPointsHandler : IRequestHandler<DeleteLastPointsQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<DeleteLastPointsHandler>();
    private readonly AnnotationHandler _annotationHandler;

    public DeleteLastPointsHandler(AnnotationHandler annotationHandler)
    {
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
    }

    public async Task<bool> Handle(DeleteLastPointsQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;

            var allAnnots = await _annotationHandler.GetAllAnnotations();
            var currentAnnot = allAnnots.FirstOrDefault(p => p.State == StateAnnot.Edit);
            if (currentAnnot is null)
                return false;

            var labelPattern = currentAnnot.LabelPattern;
            currentAnnot = DeletePoint(labelPattern, currentAnnot);

            var otherAnnots = allAnnots
                .Where(p => p.State != StateAnnot.Edit);
            var saveAnnots = otherAnnots.Append(currentAnnot).ToArray();

            await _annotationHandler.UpdateAllAnnotations(saveAnnots);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[DeleteLastPointsHandler] {@Exception}", e);
        }


        return false;
    }

    private Annotation DeletePoint(TypeLabel labelPattern, Annotation annotation)
    {
        return labelPattern switch
        {
            TypeLabel.None => throw new ArgumentOutOfRangeException(nameof(labelPattern), labelPattern, null),
            TypeLabel.Box => DeleteInBox(annotation),
            TypeLabel.Polygon => DeleteInPolygon(annotation),
            TypeLabel.PolyLine => AddInPolyline(annotation),
            TypeLabel.Point => DeleteInPoint(annotation),
            _ => throw new ArgumentOutOfRangeException(nameof(labelPattern), labelPattern, null)
        };
    }

    private Annotation DeleteInPoint(Annotation annotation)
    {
        return DeleteInBox( annotation);
    }

    private Annotation AddInPolyline(Annotation annotation)
    {
        return DeleteInBox( annotation);
    }

    private Annotation DeleteInPolygon(Annotation annotation)
    {
        return DeleteInBox( annotation);
    }

    private Annotation DeleteInBox(Annotation annotation)
    {
        if (annotation.Points == null || annotation.Points.Any() is false)
            return annotation;
        
        var lastPosition = annotation.Points.MaxBy(p => p.PositionInGroup)?.PositionInGroup ?? 0;
        var retValuePoints = annotation.Points.Where(p => p.PositionInGroup != lastPosition).ToList();
        annotation.Points = retValuePoints;
        
        return annotation;
    }
}