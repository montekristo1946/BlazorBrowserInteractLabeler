using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Устанавливает активный Label.
/// </summary>
public class SetActiveLabelHandler : IRequestHandler<SetActiveLabelQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<SetActiveLabelHandler>();
    private readonly MarkupData _markupData;
    private readonly AnnotationHandler _annotationHandler;

    public SetActiveLabelHandler(MarkupData markupData, AnnotationHandler annotationHandler)
    {
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
    }

    public async Task<bool> Handle(SetActiveLabelQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;

            _markupData.CurrentLabelId = request.IdLabel;

            var allAnnots = await _annotationHandler.GetAllAnnotations();

            if (allAnnots.Any() is false)
                return false;

            await SetIdLabelInActiveAnnot(allAnnots, _markupData.CurrentLabelId);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[HiddenAnnotHandler] {@Exception}", e);
        }
        
        return false;
    }

    private async Task SetIdLabelInActiveAnnot(Annotation[] allAnnots, int labelId)
    {
        var isUpdate = false;
        foreach (var annotation in allAnnots)
        {
            if (annotation.State != StateAnnot.Edit)
                continue;

            annotation.LabelId = labelId;
            isUpdate = true;
        }

        if (isUpdate)
        {
            await _annotationHandler.UpdateAllAnnotations(allAnnots);
        }
    }
}