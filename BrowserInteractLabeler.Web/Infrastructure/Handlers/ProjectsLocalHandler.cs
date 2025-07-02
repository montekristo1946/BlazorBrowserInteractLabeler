using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Pages;
using BrowserInteractLabeler.Web.Infrastructure.Configs;
using BrowserInteractLabeler.Web.Pages;
using Newtonsoft.Json;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Web.Infrastructure.Handlers;

public class ProjectsLocalHandler
{
    private readonly ServiceConfigs _serviceConfigs;
    private readonly ILogger _logger = Log.ForContext<MarkerModel>();
    private readonly IRepository _repository;
    private readonly NavigationHandler _navigationHandler;
    private readonly Helper _helper;
    internal SizeF RootWindowsSize => _imageWindowsSize;

    private SizeF _imageWindowsSize { get; set; } = new() { Width = 1600, Height = 800 };

    // private SizeF _imageWindowsSize { get; set; }= new() { Width = 1600, Height = 800 };
    internal bool LoadingDB { get; set; } = false;
    // internal string GetHeightSqlSelectorPanel =>$"{(int)_imageWindowsSize.Height}px";

    internal string CurrentSqlDbName { get; set; } = String.Empty;
    internal string CurrentInformationSqlDb { get; set; } = String.Empty;


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
        // if (arg is null && !arg.Any())
        //     return;
        // LoadingDB = true;
        // CurrentSqlDbName = arg;
        //
        // if (!File.Exists(arg))
        //     return;
        //
        // var res = _repository.LoadDatabase(arg);
        // if (!res)
        // {
        //     _logger.Error("[HandlerChoseActiveDataBaseAsync] fail LoadDatabaseAsync {PathDb}", arg);
        //     return;
        // }
        //
        // await LoadInformationOnStateDb();
        // await _navigationHandler.GetDbName(CurrentSqlDbName);
        // await _navigationHandler.LoadFirstImg();
        // LoadingDB = false;
        throw new NotImplementedException();
    }

    private async Task LoadInformationOnStateDb()
    {
        // var allInformation = _repository.GetInformationDto();
        // var currentInfo = allInformation.Where(p => p.CategoryInformation == 1);
        // var lastInfo = currentInfo.MaxBy(p => p.Id);
        // CurrentInformationSqlDb = lastInfo is not null ? lastInfo.Information : "Being processed ...";
        throw new NotImplementedException();
    }

    internal async Task HandlerChoseExportDataBaseAsync(string fullPathDb)
    {
        if (string.IsNullOrEmpty(fullPathDb))
            return;

        throw new NotImplementedException();
        // try
        // {
        //     var labels = _repository.GetAllLabels();
        //     var frames = _repository.GetAllImages();
        //     var annots = _repository.GetAllAnnotations();
        //     var saveJson = new ExportDTO()
        //     {
        //         Labels = labels,
        //         Annotations = annots,
        //         Images = frames
        //     };
        //
        //     var jsonSerializerSettings = new JsonSerializerSettings();
        //     jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        //
        //     var json = JsonConvert.SerializeObject(saveJson, jsonSerializerSettings);
        //     var jsonPath = Path.Combine(_serviceConfigs.ExportCompletedTasks, $"{Path.GetFileName(fullPathDb)}.json");
        //     await File.WriteAllTextAsync(jsonPath, json);
        //
        //     var newInformationDto = new InformationDto()
        //     {
        //         Information = "Completed",
        //         CategoryInformation = 1,
        //     };
        //
        //     var resSaveInformationDtoAsync = _repository.SaveInformationDto(newInformationDto);
        //     if (!resSaveInformationDtoAsync)
        //         _logger.Error("[HandlerChoseExportDataBaseAsync] Export fail SaveInformationDtoAsync {PathDb}",
        //             fullPathDb);
        //
        //     await LoadInformationOnStateDb();
        // }
        // catch (Exception e)
        // {
        //     _logger.Error("[HandlerChoseExportDataBaseAsync] Export fail: {PathDb}", fullPathDb);
        // }
    }

    internal void SetRootWindowsSize(SizeF sizeBrowse)
    {
        // _imageWindowsSize = _helper.CalculationRootWindowsSize(sizeBrowse);
        _imageWindowsSize = sizeBrowse;
    }
}