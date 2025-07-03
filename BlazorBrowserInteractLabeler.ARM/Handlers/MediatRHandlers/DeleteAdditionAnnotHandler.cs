using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Удаляет annot который находится в состоянии редактируется.
/// </summary>
public class DeleteAdditionAnnotHandler : IRequestHandler<DeleteEditionAnnotQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<DeleteAdditionAnnotHandler>();
    private readonly AnnotationHandler _annotationHandler;

    public DeleteAdditionAnnotHandler(AnnotationHandler annotationHandler)
    {
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
    }

    public async Task<bool> Handle(DeleteEditionAnnotQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;

            var allAnnots = await _annotationHandler.GetAllAnnotations();

            if (allAnnots.Any() is false)
                return false;

            allAnnots = allAnnots
                .Where(p => p.State != StateAnnot.Edit)
              .ToArray();
            
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