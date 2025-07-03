using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

public class HiddenAllAnnotHandler : IRequestHandler<HiddenAllAnnotQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<HiddenAllAnnotHandler>();
    private readonly AnnotationHandler _annotationHandler;

    public HiddenAllAnnotHandler(AnnotationHandler annotationHandler)
    {
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
    }

    public async Task<bool> Handle(HiddenAllAnnotQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;

            var allAnnots = await _annotationHandler.GetAllAnnotations();

            if (allAnnots.Any() is false)
                return false;

            foreach (var annotation in allAnnots)
            {
                annotation.State = request.IsHidden ? StateAnnot.Hidden: StateAnnot.Finalized;
            }
            await _annotationHandler.UpdateAllAnnotations(allAnnots);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[HiddenAllAnnotHandler] {@Exception}", e);
        }


        return false;
    }
}