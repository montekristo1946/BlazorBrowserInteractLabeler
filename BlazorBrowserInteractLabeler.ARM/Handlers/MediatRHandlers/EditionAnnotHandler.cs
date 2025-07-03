using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Устанавливает состояние редактируется для конкретного annot
/// </summary>
public class EditionAnnotHandler : IRequestHandler<EditionAnnotQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<EditionAnnotHandler>();
    private readonly AnnotationHandler _annotationHandler;

    public EditionAnnotHandler(AnnotationHandler annotationHandler)
    {
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
    }

    public async Task<bool> Handle(EditionAnnotQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;

            var allAnnots = await _annotationHandler.GetAllAnnotations();

            if (allAnnots.Any() is false)
                return false;

            var annot = allAnnots.FirstOrDefault(p => p.Id == request.IdAnnotaion);

            if (annot is null)
                return false;

            allAnnots = allAnnots
                .Where(p => p.Id != request.IdAnnotaion)
                .Select(p =>
                {
                    if (p.State == StateAnnot.Edit)
                        p.State = StateAnnot.Finalized;
                    return p;
                }).ToArray();
            
            annot.State = annot.State != StateAnnot.Edit ? StateAnnot.Edit : StateAnnot.Finalized;
            allAnnots = allAnnots.Append(annot).ToArray();

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