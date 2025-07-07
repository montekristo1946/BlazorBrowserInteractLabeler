using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Сохранить конфигурацию утилиты на диск.
/// </summary>
public class LoadConfigurationHandler: IRequestHandler<LoadConfigurationQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<LoadConfigurationHandler>();
    private readonly IMediator _mediator;
    private readonly SettingsData _settingsData;

    public LoadConfigurationHandler(SettingsData settingsData, IMediator mediator)
    {
        _settingsData = settingsData ?? throw new ArgumentNullException(nameof(settingsData));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(LoadConfigurationQueries request, CancellationToken cancellationToken)
    {
        try
        {
            var pathSave = _settingsData.PathDirConfigs;

            if (!File.Exists(pathSave))
            {
                _settingsData.Init();
                await _mediator.Send(new SaveConfigurationQueries(), cancellationToken);
                return false;
            }

            var arrText = await File.ReadAllTextAsync(pathSave, cancellationToken);

            var configNew = JsonConvert.DeserializeObject<SettingsData>(arrText);
            if (configNew is null)
                throw new Exception($"[LoadConfigurationHandler] fail DeserializeObject {pathSave}");

            FildConfig(configNew,_settingsData);
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[SaveConfigurationHandler] {@Exception}", e);
        }
        
        return false;
    }

    private void FildConfig(SettingsData source, SettingsData  distination)
    {
        distination.PathFolderWorkers = source.PathFolderWorkers;
        distination.StrokeWidth = source.StrokeWidth;
        distination.CodeKey = source.CodeKey;
        distination.ColorModel = source.ColorModel;
    }
}