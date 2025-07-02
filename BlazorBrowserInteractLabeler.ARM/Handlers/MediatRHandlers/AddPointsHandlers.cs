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
public class AddPointsHandlers : IRequestHandler<AddPointsQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<AddPointsHandlers>();
    private readonly AnnotationHandler _annotationHandler;

    public AddPointsHandlers( AnnotationHandler annotationHandler)
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
        switch (tabelPattern)
        {
            case TypeLabel.None:
                throw new ArgumentOutOfRangeException(nameof(tabelPattern), tabelPattern, null);
            case TypeLabel.Box:
                return AddInBox(annotation, point);

            case TypeLabel.Polygon:
                break;
            case TypeLabel.PolyLine:
                break;
            case TypeLabel.Point:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(tabelPattern), tabelPattern, null);
        }

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

        annotation.Points.Add(new PointD()
        {
            X = point.X,
            Y = point.Y,
        });
        return annotation;
    }
}