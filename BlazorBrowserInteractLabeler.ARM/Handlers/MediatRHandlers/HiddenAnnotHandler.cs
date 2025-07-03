using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Устанавливает состояние скрытый для конкретного annot
/// </summary>
public class HiddenAnnotHandler : IRequestHandler<HiddenAnnotQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<HiddenAllAnnotHandler>();
    private readonly AnnotationHandler _annotationHandler;

    public HiddenAnnotHandler(AnnotationHandler annotationHandler)
    {
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
    }

    public async Task<bool> Handle(HiddenAnnotQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;

            var allAnnots = await _annotationHandler.GetAllAnnotations();

            if (allAnnots.Any() is false)
                return false;

            var annot = allAnnots.FirstOrDefault(p=>p.Id == request.IdAnnotaion);
            
            if(annot is null)
                return false;

            annot.State = annot.State == StateAnnot.Hidden ? StateAnnot.Finalized : StateAnnot.Hidden;
            
            await _annotationHandler.UpdateAllAnnotations(allAnnots);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[HiddenAnnotHandler] {@Exception}", e);
        }


        return false;
    }
}