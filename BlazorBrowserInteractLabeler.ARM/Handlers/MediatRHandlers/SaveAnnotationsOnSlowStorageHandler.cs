using BlazorBrowserInteractLabeler.ARM.Extension;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Сохранить Annots в Sql хранилище.
/// </summary>
public class SaveAnnotationsOnSlowStorageHandler:IRequestHandler<SaveAnnotationsOnSlowStorageQueries,bool>
{
    private readonly ILogger _logger = Log.ForContext<LoadNextImageHandler>();
    private readonly IRepository _repository;

    private readonly AnnotationHandler _annotationHandler;

    public SaveAnnotationsOnSlowStorageHandler(IRepository repository, AnnotationHandler annotationHandler)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
    }

    public async Task<bool> Handle(SaveAnnotationsOnSlowStorageQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;

            var indexImg = request.ImageId;
            var removeAnnot =await _repository.GetAnnotationsFromImgIdAsync(indexImg);
            var annotationsFromCash = await _annotationHandler.GetAllAnnotations();
           
          
            var equalAnnotation = removeAnnot.Equality(annotationsFromCash);
            if (equalAnnotation)
                return true;

            _logger.Debug("[SaveAnnotationsOnSlowStorageHandler] " +
                          "Save annotations in Img:{imagesId} count annotations:{CountAnnotations}", indexImg,
                annotationsFromCash.Count());


            await _repository.DeleteAnnotationsAsync(removeAnnot);
            annotationsFromCash = ClearFailAnnotation(annotationsFromCash);
            annotationsFromCash = OrderPoints(annotationsFromCash);
            await _repository.SaveAnnotationsAsync(annotationsFromCash);

            var allAnnotWithIndex = await _repository.GetAnnotationsFromImgIdAsync(indexImg);
            
            await _annotationHandler.UpdateAllAnnotations(allAnnotWithIndex);
            
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[LoadNextImageHandler] {@Exception}", e);
        }

        return false;
    }
    
    private Annotation[] OrderPoints(Annotation[] annotations)
    {
        if (annotations?.Any() == null)
            return [];

        var retArr = annotations.Select(annot =>
        {
            var newPoints = annot.Points
                ?.Select((point, index) => point with { Id = 0, PositionInGroup = index })
                .ToList();
            return annot with { Points = newPoints };
        }).ToArray();

        return retArr;
    }
    
    private Annotation[] ClearFailAnnotation(Annotation[] annotations)
    {
        if (annotations?.Any() == null)
            return [];

        var retAnnots = annotations.Where(annot => annot.Points?.Any() != null)
            .Where(annot =>
                (annot.LabelPattern == TypeLabel.Box && annot.Points.Count == 2)
                || (annot.LabelPattern == TypeLabel.PolyLine && annot.Points.Count >= 2)
                || (annot.LabelPattern == TypeLabel.Polygon && annot.Points.Count >= 3)
                || (annot.LabelPattern == TypeLabel.Point && annot.Points.Count >= 1)
                 
            ).Where(annot => annot.LabelId >=0 )
            .Select(annot =>
            {
                annot.State = StateAnnot.Finalized;
                annot.Id = 0;
                return annot;
            }).ToArray();

        return retAnnots;
    }
}