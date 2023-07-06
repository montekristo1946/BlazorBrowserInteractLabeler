using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Infrastructure.Configs;
using BrowserInteractLabeler.Pages;
using Newtonsoft.Json;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Infrastructure;

public class ProjectsLocalHandler
{
    private readonly ServiceConfigs _serviceConfigs;
    private readonly ILogger _logger = Log.ForContext<MarkerModel>();
    private readonly IRepository _repository;
    private readonly NavigationHandler _navigationHandler;
    private readonly Helper _helper;
    private SizeF _imageWindowsSize { get; set; }= new() { Width = 1600, Height = 800 };
    internal bool LoadingDB { get; set; } = false;
    internal string GetHeightSqlSelectorPanel =>$"{(int)_imageWindowsSize.Height}px";

    internal string CurrentSqlDbName { get; set; } = String.Empty;
    // internal string GetCurrentSqlDbNames() => _serviceConfigs.LastLoadDb;


    public ProjectsLocalHandler(ServiceConfigs serviceConfigs,
        IRepository repository,
        NavigationHandler navigationHandler,
        Helper helper)
    {
        _serviceConfigs = serviceConfigs ?? throw new ArgumentNullException(nameof(serviceConfigs));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _navigationHandler = navigationHandler ?? throw new ArgumentNullException(nameof(navigationHandler));
        _helper = helper ?? throw new ArgumentNullException(nameof(helper));
    }

    internal string[] GetAllSqlDataBase()
    {
        var pathSqlDb = _serviceConfigs.PathSqlDb;
        if (!Path.Exists(pathSqlDb))
            return Array.Empty<string>();
            
        var allDbPaths = Directory.GetFiles(pathSqlDb, "*.db3", SearchOption.AllDirectories);
        Array.Sort(allDbPaths);

        return allDbPaths;
    }

    internal async Task HandlerChoseActiveDataBaseAsync(string arg)
    {
        if (arg is null && !arg.Any())
            return;
        LoadingDB = true;
        CurrentSqlDbName = arg;

        if (!File.Exists(arg))
            return;

        var res = await _repository.LoadDatabaseAsync(arg);
        if (!res)
        {
            _logger.Error("[HandlerChoseActiveDataBaseAsync] fail LoadDatabaseAsync {PathDb}", arg);
            return;
        }

        await _navigationHandler.LoadFirstImg();
        LoadingDB = false;
    }

    internal async Task HandlerChoseExportDataBaseAsync(string fullPathDb) //TODO: еще не сделал
    {
        if(fullPathDb is null || !fullPathDb.Any())
            return;

        try
        {
            var labels = await _repository.GetAllLabelsAsync();
            var frames = await _repository.GetAllImagesAsync();
            var annots = await _repository.GetAllAnnotationsAsync();
            var saveJson = new ExportDTO()
            {
                Labels = labels,
                Annotations = annots,
                Images = frames
            };
            
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            var json = JsonConvert.SerializeObject(saveJson,jsonSerializerSettings);
            var jsonPath = Path.Combine(_serviceConfigs.ExportCompletedTasks, $"{Path.GetFileName(fullPathDb)}.json");
            await File.WriteAllTextAsync(jsonPath, json);
        }
        catch (Exception e)
        {
            _logger.Error("[HandlerChoseExportDataBaseAsync] Export fail: {PathDb}", fullPathDb);
        }
 


        
    }

    internal async Task SetRootWindowsSize(SizeF sizeBrowse)
    {
        _imageWindowsSize = _helper.CalculationRootWindowsSize(sizeBrowse);
    }
}