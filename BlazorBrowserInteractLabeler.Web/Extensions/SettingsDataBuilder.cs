using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.ViewData;

namespace BlazorBrowserInteractLabeler.Web.Extensions;

public static class SettingsDataBuilder
{
    public static SettingsData Build(IServiceProvider serviceProvider)
    {
        var retData = new SettingsData();
        retData.Init();

        return retData;
    }
}