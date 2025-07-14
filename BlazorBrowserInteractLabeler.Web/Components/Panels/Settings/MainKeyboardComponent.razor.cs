using BlazorBrowserInteractLabeler.ARM.Extension;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Settings;

public partial class MainKeyboardComponent : ComponentBase
{
    [Inject] private SettingsData SettingsData { get; set; } = null!;
    [Inject] private Mappers Mappers { get; set; } = null!;

    [Inject] private IMediator Mediator { get; set; } = null!;
    [Parameter] public CodeKey? CodeKey { get; set; } = null!;

    [Parameter] public Action? IsNeedUpdateUi { get; set; } = null!;

    private async Task OnClickDeleteAnnot(CodeKey? codeKey)
    {
        if (codeKey is null)
            return;

        await Mediator.Send(new DeleteCodeKeyQueries() { CodeKey = codeKey });

        IsNeedUpdateUi?.Invoke();
    }

    private string GetBackground()
    {
        if (CodeKey is null)
            return string.Empty;

        var id = Mappers.MapEventCodeToIdLabel(CodeKey.EventCode);
        if (id == 0)
            return string.Empty;

        var colors = SettingsData.ColorModel;
        var retColor = colors.FirstOrDefault(p => p.IdLabel == id)?.Color ?? string.Empty;

        return retColor;
    }
}