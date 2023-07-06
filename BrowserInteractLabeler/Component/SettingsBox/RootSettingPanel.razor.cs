using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Infrastructure.Configs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BrowserInteractLabeler.Component.SettingsBox;

public class RootSettingPanelModel: ComponentBase
{
    
    [Parameter] public ServiceConfigs Configs { get; set; }
    
    [Parameter] public EventCallback<ServiceConfigs> UpdateServiceConfigs { get; set; }
 
    internal async Task ButtonClickSaveAsync(MouseEventArgs arg)
    {
        await UpdateServiceConfigs.InvokeAsync(Configs);
    }
    
 
}