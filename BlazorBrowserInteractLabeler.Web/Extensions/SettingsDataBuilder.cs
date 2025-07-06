using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.ViewData;

namespace BlazorBrowserInteractLabeler.Web.Extensions;

public static class SettingsDataBuilder
{
    public static SettingsData Build(IServiceProvider serviceProvider)
    {
        var retData = new SettingsData();
        retData.Init();
        retData.PathFolderWorkers = "/mnt/Disk_D/TMP/17.06.2025/";
        return retData;
    }
}