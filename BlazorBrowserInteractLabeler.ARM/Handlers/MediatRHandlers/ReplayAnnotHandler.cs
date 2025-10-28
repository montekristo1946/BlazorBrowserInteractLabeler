using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

public class ReplayAnnotHandler: IRequestHandler<ReplayAnnotQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<ReplayAnnotHandler>();
    private readonly AnnotationHandler _annotationHandler;
    private readonly MarkupData _markupData;

    public ReplayAnnotHandler(AnnotationHandler annotationHandler, MarkupData markupData)
    {
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
    }

    public async Task<bool> Handle(ReplayAnnotQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null || request.IdImage <=0 || request.IdImage>_markupData.AllImagesCount)
                return false;
            
            var annot = _markupData.LastAnnot;
            if (annot is null)
                return false;
                    
                    
            var allAnnots = await _annotationHandler.GetAllAnnotations();
            foreach (var annotation in allAnnots.Where(annotation => annotation.State != StateAnnot.Hidden))
            {
                annotation.State = StateAnnot.Finalized;
            }
            
            var annotInImg = allAnnots.MaxBy(p => p.Id);
            var currentDb = 1;

            if (annotInImg is not null)
                currentDb = annotInImg.Id + 1;
            
            annot.State = StateAnnot.Edit;
            annot.Id = currentDb;
            annot.ImageFrameId = request.IdImage;
            
            allAnnots = allAnnots.Append(annot).ToArray();

            await _annotationHandler.UpdateAllAnnotations(allAnnots);
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[ReplayAnnotHandler] {@Exception}", e);
        }
        
        return false;
    }
}