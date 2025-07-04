using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Добавит новый инстанс Annot в хранилище Annots.
/// </summary>
public class InitNewAnnotHandler:IRequestHandler<InitNewAnnotQueries,bool>
{
    private readonly AnnotationHandler _annotationHandler;
    private readonly MarkupData _markupData;
    private readonly ILogger _logger = Log.ForContext<InitNewAnnotHandler>();

    public InitNewAnnotHandler(AnnotationHandler annotationHandler, MarkupData markupData)
    {
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
        _markupData = markupData;
    }

    public async Task<bool> Handle(InitNewAnnotQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;
            
            var indexImg = _markupData.CurrentIdImg;
            var typeLabel = request.TypeLabel;
            var labelId = _markupData.CurrentLabelId;
          
            var allAnnots = await _annotationHandler.GetAllAnnotations();
           
            foreach (var annotation in allAnnots.Where(annotation => annotation.State != StateAnnot.Hidden))
            {
                annotation.State = StateAnnot.Finalized;
            }
            
            var lastAnnot = allAnnots.MaxBy(p => p.Id);
            var currentDb = 1;

            if (lastAnnot is not null)
                currentDb = lastAnnot.Id + 1;

            var annot = new Annotation()
            {
                Id = currentDb,
                Points = [],
                ImageFrameId = indexImg,
                State = StateAnnot.Edit,
                LabelId = labelId,
                LabelPattern = typeLabel
            };
            allAnnots = allAnnots.Append(annot).ToArray();

            await _annotationHandler.UpdateAllAnnotations(allAnnots);
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[InitNewAnnotHandler] {@Exception}", e);
        }
        
        return false;
    }
}