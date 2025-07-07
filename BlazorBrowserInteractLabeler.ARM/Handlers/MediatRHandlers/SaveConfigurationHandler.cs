using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Сохранить конфигурацию утилиты на диск.
/// </summary>
public class SaveConfigurationHandler: IRequestHandler<SaveConfigurationQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<SaveConfigurationHandler>();
    private readonly SettingsData _settingsData;

    public SaveConfigurationHandler(SettingsData settingsData)
    {
        _settingsData = settingsData ?? throw new ArgumentNullException(nameof(settingsData));
    }

    public async Task<bool> Handle(SaveConfigurationQueries request, CancellationToken cancellationToken)
    {
        try
        {
            var confgi = _settingsData;
            var pathSave = _settingsData.PathDirConfigs;
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            var json = JsonConvert.SerializeObject(confgi, jsonSerializerSettings);
            await File.WriteAllTextAsync(pathSave, json, cancellationToken);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[SaveConfigurationHandler] {@Exception}", e);
        }
        
        return false;
    }
}