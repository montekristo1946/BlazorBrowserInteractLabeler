using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
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
    private readonly MarkupData _markupData;

    public EditionAnnotHandler(AnnotationHandler annotationHandler, MarkupData markupData)
    {
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
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
            _markupData.CurrentTypeLabel = annot.LabelPattern;
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[HiddenAnnotHandler] {@Exception}", e);
        }


        return false;
    }
}