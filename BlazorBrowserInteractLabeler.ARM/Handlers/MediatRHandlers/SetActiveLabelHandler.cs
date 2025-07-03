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

    public SetActiveLabelHandler(MarkupData markupData)
    {
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
    }

    public async Task<bool> Handle(SetActiveLabelQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;

            _markupData.CurrentLabelId = request.IdLabel;

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[HiddenAnnotHandler] {@Exception}", e);
        }


        return false;
    }
}