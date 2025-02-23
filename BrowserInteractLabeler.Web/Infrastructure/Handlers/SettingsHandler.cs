using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Web.Infrastructure.Configs;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Web.Infrastructure.Handlers;

public class SettingsHandler
{
    // internal string GetHeightSettingsPanel =>$"{(int)_imageWindowsSize.Height}px";

    internal SizeF RootWindowsSize => _imageWindowsSize;

    // private readonly Helper _helper;
    private readonly ILogger _logger = Log.ForContext<SettingsHandler>();
    private SizeF _imageWindowsSize { get; set; } = new() { Width = 1600, Height = 800 };
    private readonly ServiceConfigs _serviceConfigs;


    public SettingsHandler(ServiceConfigs serviceConfigs)
    {
        // _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        _serviceConfigs = serviceConfigs ?? throw new ArgumentNullException(nameof(serviceConfigs));

    }
    internal void SetRootWindowsSize(SizeF sizeBrowse)
    {
        // _imageWindowsSize = _helper.CalculationRootWindowsSize(sizeBrowse);
        _imageWindowsSize = sizeBrowse;
    }

    public async Task UpdateColorModelAsync(ColorModel arg)
    {
        var otherColorModel = _serviceConfigs.Colors.Where(p => p.IdLabel != arg.IdLabel).ToList();
        otherColorModel.Add(arg);
        _serviceConfigs.Colors = otherColorModel.ToArray();
        ServiceConfigs.SaveInFile(_serviceConfigs);
    }

    public Task UpdateServiceConfigsAsync(ServiceConfigs arg)
    {
        _serviceConfigs.ExportCompletedTasks = arg.ExportCompletedTasks;
        _serviceConfigs.PathSqlDb = arg.PathSqlDb;
        ServiceConfigs.SaveInFile(_serviceConfigs);
        return Task.CompletedTask;
    }
}