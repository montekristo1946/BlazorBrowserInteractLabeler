using BrowserInteractLabeler.Web.Infrastructure.Configs;
using BrowserInteractLabeler.Web.Infrastructure.Handlers;
using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Web.Pages;

public class SettingsPageModel : ComponentBase
{
    [Inject] internal SettingsHandler _settingsHandler { get; set; }
    [Inject] internal ServiceConfigs _serviceConfigs { get; set; }

}