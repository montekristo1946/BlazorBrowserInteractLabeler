using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

public class ExportDatabaseHandler: IRequestHandler<ExportDatabaseQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<ExportDatabaseHandler>();
    private readonly IRepository _repository;
    private readonly SettingsData _settingsData;
    private readonly MarkupData _markupData;

    public ExportDatabaseHandler(IRepository repository, SettingsData settingsData, MarkupData markupData)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _settingsData = settingsData ?? throw new ArgumentNullException(nameof(settingsData));
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
    }

    public async Task<bool> Handle(ExportDatabaseQueries request, CancellationToken cancellationToken)
    {
        try
        {
            var labels = await _repository.GetAllLabelsAsync();
            var frames = await _repository.GetInfoAllImagesAsync();
            var annots =await  _repository.GetAllAnnotationsAsync();
            var saveJson = new ExportDTO()
            {
                Labels = labels,
                Annotations = annots,
                Images = frames
            };
            
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            
            var json = JsonConvert.SerializeObject(saveJson, jsonSerializerSettings);
            var jsonPath = Path.Combine(_settingsData.PathFolderWorkers, $"{Path.GetFileName(_markupData.NameDb)}.json");
            await File.WriteAllTextAsync(jsonPath, json, cancellationToken);
            
            
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[ExportDatabaseHandler] {@Exception}", e);
        }

        return false;
    }
}