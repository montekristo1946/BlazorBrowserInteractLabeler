using BrowserInteractLabeler.Common;
using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Web.Component.SettingsBox;

public class TabSettingsPanelModel : ComponentBase
{
    [Parameter] public RenderFragment ColorMapPanelTemplate { get; set; }
    [Parameter] public RenderFragment RootSettingTemplate { get; set; }

    [Parameter] public SizeF RootWindowsSize { get; set; }


    internal string GetHeightPanel()
    {
        const double coef = 0.88d;
        return $"{RootWindowsSize.Height * coef}px";
    }
}