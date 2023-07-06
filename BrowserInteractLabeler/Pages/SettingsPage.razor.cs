using BrowserInteractLabeler.Infrastructure;
using BrowserInteractLabeler.Infrastructure.Configs;
using Microsoft.AspNetCore.Components;


namespace BrowserInteractLabeler.Pages;

public class SettingsPageModel:ComponentBase
{
    [Inject] internal SettingsHandler _settingsHandler { get; set; }
    [Inject] internal ServiceConfigs _serviceConfigs { get; set; }
    
}