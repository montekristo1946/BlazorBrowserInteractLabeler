using BlazorBrowserInteractLabeler.ARM.ViewData;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Settings;

public partial class SettingComponent : ComponentBase
{
    [Inject] private SettingsData SettingsData { get; set; } = null!;



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

    private RenderFragment GetMainKeyboardComponentTemplate(CodeKey codeKey) => (builder) =>
    {
        builder.OpenComponent(0, typeof(MainKeyboardComponent));
        builder.AddAttribute(1, nameof(MainKeyboardComponent.CodeKey), codeKey);
        builder.AddAttribute(2, nameof(MainKeyboardComponent.IsNeedUpdateUi), UpdateUiMainKeyboardComponent);

        builder.CloseComponent();
    };

    private RenderFragment CreateKeyboardComponentTemplate() => (builder) =>
    {
        builder.OpenComponent(0, typeof(CreateKeyboardComponent));
        builder.AddAttribute(2, nameof(CreateKeyboardComponent.IsNeedUpdateUi), UpdateUiMainKeyboardComponent);
        builder.CloseComponent();
    };

    private void UpdateUiMainKeyboardComponent()
    {

        StateHasChanged();
    }

    private IEnumerable<CodeKey> GetKeyCode()
    {

        var keys = SettingsData.CodeKey.OrderBy(p => p.EventCode).ToArray();
        return keys;
    }
}