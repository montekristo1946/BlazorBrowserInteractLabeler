using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Component.SettingsBox;

public class TabSettingsPanelModel : ComponentBase
{
    [Parameter] public RenderFragment ColorMapPanelTemplate { get; set; }
    [Parameter] public RenderFragment RootSettingTemplate  { get; set; }
    
    [Parameter] public string HeightPanel { get; set; }
}