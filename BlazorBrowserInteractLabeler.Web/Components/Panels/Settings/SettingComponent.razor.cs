using BlazorBrowserInteractLabeler.ARM.ViewData;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Settings;

public partial class SettingComponent : ComponentBase
{
    [Inject] private SettingsData _settingsData { get; set; } = null!;

    private CodeKey[] _codeKeys = [];

    protected override void OnInitialized()
    {
        _codeKeys = _settingsData.CodeKey;
    }

    private Task SetStokeWidth(ChangeEventArgs changeEventArgs)
    {
        var textToSave = (string)changeEventArgs.Value! ?? string.Empty;
        var newValue = double.TryParse(textToSave, out double parsValue)
            ? parsValue
            : 0;

        _settingsData.StrokeWidth = newValue;
        
       return Task.CompletedTask;
    }

    private Task SetPathFolderWorkers(ChangeEventArgs changeEventArgs)
    {
        var textToSave = (string)changeEventArgs.Value! ?? string.Empty;

        _settingsData.PathFolderWorkers = textToSave;
        
        return Task.CompletedTask;
    }

    private string GetPathFolderWorkers()
    {
        return _settingsData.PathFolderWorkers;
    }

    private double GetSetStokeWidth()
    {
        return _settingsData.StrokeWidth;
    }
}