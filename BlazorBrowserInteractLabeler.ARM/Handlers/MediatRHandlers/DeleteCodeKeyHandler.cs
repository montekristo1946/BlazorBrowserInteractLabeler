using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

public class DeleteCodeKeyHandler: IRequestHandler<DeleteCodeKeyQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<DeleteCodeKeyHandler>();
    private readonly SettingsData _settingsData;

    public DeleteCodeKeyHandler(SettingsData settingsData)
    {
        _settingsData = settingsData ?? throw new ArgumentNullException(nameof(settingsData));
    }

    public Task<bool> Handle(DeleteCodeKeyQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null || request.CodeKey is null)
                return Task.FromResult(false);

            var deleteCodeFromKeyBoard = request.CodeKey.CodeFromKeyBoard;
         
            var keysSrc = _settingsData.CodeKey;
            var sortKey = keysSrc.Where(p => p.CodeFromKeyBoard != deleteCodeFromKeyBoard).ToArray();
            if (sortKey.Length == keysSrc.Length)
                return Task.FromResult(false);

            _settingsData.CodeKey = sortKey;

            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            _logger.Error("[DeleteCodeKeyHandler] {@Exception}", e);
        }

        return Task.FromResult(false);
    }
}