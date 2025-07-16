using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Settings;

public partial class SettingComponent : ComponentBase
{
    [Inject] private SettingsData SettingsData { get; set; } = null!;

    [Inject] private IMediator Mediator { get; set; } = null!;

    private Task SetStokeWidth(ChangeEventArgs changeEventArgs)
    {
        var textToSave = (string)changeEventArgs.Value! ?? string.Empty;
        var newValue = double.TryParse(textToSave, out double parsValue)
            ? parsValue
            : 0;

        SettingsData.StrokeWidth = newValue;

        return Task.CompletedTask;
    }

    private async Task SetPathFolderWorkers(ChangeEventArgs changeEventArgs)
    {
        var textToSave = (string)changeEventArgs.Value! ?? string.Empty;

        SettingsData.PathFolderWorkers = textToSave;

        await Mediator.Send(new UnloadDataBaseQueries());
    }

    private string GetPathFolderWorkers()
    {
        return SettingsData.PathFolderWorkers;
    }

    private double GetStokeWidth()
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

    private double GetCoefResizeView()
    {
        return SettingsData.CoefResizeView;
    }

    private void SetCoefResizeView(ChangeEventArgs changeEventArgs)
    {
        var textToSave = (string)changeEventArgs.Value! ?? string.Empty;
        var newValue = double.TryParse(textToSave, out double parsValue)
            ? parsValue
            : 0;

        SettingsData.CoefResizeView = newValue;
        
    }
}