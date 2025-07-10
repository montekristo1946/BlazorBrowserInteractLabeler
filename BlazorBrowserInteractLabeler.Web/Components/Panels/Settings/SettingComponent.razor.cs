using BlazorBrowserInteractLabeler.ARM.ViewData;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Settings;

public partial class SettingComponent : ComponentBase
{
    [Inject] private SettingsData SettingsData { get; set; } = null!;

    // private CodeKey[] _codeKeys = [];

    protected override void OnInitialized()
    {
        // _codeKeys = SettingsData.CodeKey;
    }

    private Task SetStokeWidth(ChangeEventArgs changeEventArgs)
    {
        var textToSave = (string)changeEventArgs.Value! ?? string.Empty;
        var newValue = double.TryParse(textToSave, out double parsValue)
            ? parsValue
            : 0;

        SettingsData.StrokeWidth = newValue;
        
       return Task.CompletedTask;
    }

    private Task SetPathFolderWorkers(ChangeEventArgs changeEventArgs)
    {
        var textToSave = (string)changeEventArgs.Value! ?? string.Empty;

        SettingsData.PathFolderWorkers = textToSave;
        
        return Task.CompletedTask;
    }

    private string GetPathFolderWorkers()
    {
        return SettingsData.PathFolderWorkers;
    }

    private double GetSetStokeWidth()
    {
        return SettingsData.StrokeWidth;
    }
    
    private RenderFragment GetMainKeyboardComponent(CodeKey codeKey) => (builder) =>
    {
        builder.OpenComponent(0, typeof(MainKeyboardComponent));
        builder.AddAttribute(1,nameof(MainKeyboardComponent.CodeKey),codeKey);
        builder.AddAttribute(2,nameof(MainKeyboardComponent.IsNeedUpdateUi),UpdateUiMainKeyboardComponent);
        
        builder.CloseComponent();
    };
    
    private RenderFragment CreateKeyboardComponent() => (builder) =>
    {
        builder.OpenComponent(0, typeof(CreateKeyboardComponent));
        builder.CloseComponent();
    };
    
    private void UpdateUiMainKeyboardComponent ()
    {
    
        StateHasChanged();
    }
}