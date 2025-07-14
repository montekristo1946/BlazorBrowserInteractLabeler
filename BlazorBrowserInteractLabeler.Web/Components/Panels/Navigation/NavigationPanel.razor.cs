using BlazorBrowserInteractLabeler.ARM.Handlers;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BlazorBrowserInteractLabeler.Web.Components.Panels.PagesSelector;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Navigation;

public partial class NavigationPanel : ComponentBase
{
    [Inject] private IMediator Mediator { get; set; } = null!;
    [Inject] private SettingsData SettingsData { get; set; } = null!;
    [Inject] private MarkupData MarkupData { get; set; } = null!;

    [Parameter] public Action? IsNeedUpdateUi { get; set; } = null!;


    private async Task OnClickBackImg()
    {
        await Mediator.Send(new LoadNextImageQueries() { IsForward = false });
        IsNeedUpdateUi?.Invoke();
    }

    private async Task OnClickNextImg()
    {
        await Mediator.Send(new LoadNextImageQueries() { IsForward = true });
        IsNeedUpdateUi?.Invoke();
    }

    private async Task OnClickSave()
    {
        await Mediator.Send(new SaveAnnotationsOnSlowStorageQueries());
        IsNeedUpdateUi?.Invoke();
    }

    private void OnClickUndo()
    {
        IsNeedUpdateUi?.Invoke();
    }

    private void OnClickRedo()
    {
        IsNeedUpdateUi?.Invoke();
    }

    private async Task OnClickInitRectangle()
    {
        await Mediator.Send(new InitNewAnnotQueries() { TypeLabel = TypeLabel.Box });
        IsNeedUpdateUi?.Invoke();
    }

    private async Task OnClickInitPolygon()
    {
        await Mediator.Send(new InitNewAnnotQueries() { TypeLabel = TypeLabel.Polygon });
        IsNeedUpdateUi?.Invoke();
    }

    private async Task OnClickInitPolyline()
    {
        await Mediator.Send(new InitNewAnnotQueries() { TypeLabel = TypeLabel.PolyLine });
        IsNeedUpdateUi?.Invoke();
    }

    private async Task OnClickInitPoints()
    {
        await Mediator.Send(new InitNewAnnotQueries() { TypeLabel = TypeLabel.Point });
        IsNeedUpdateUi?.Invoke();
    }
    private async Task ClickSetupIndexImage(ChangeEventArgs changeEventArgs)
    {
        var resultTryParse = Int32.TryParse((string?)changeEventArgs.Value, out var indexImg);
        if (!resultTryParse)
            return;

        await Mediator.Send(new LoadByIndexImageQueries() { IndexImage = indexImg });
        IsNeedUpdateUi?.Invoke();
    }

    private async Task OnClickRestorePositionImage()
    {
        await Mediator.Send(new RestorePositionImageQueries());
        IsNeedUpdateUi?.Invoke();
    }

    private string GetBacgroundLabel()
    {
        var labelId = MarkupData.CurrentLabelId;
        var colorModels = SettingsData.ColorModel;
        var color = colorModels.FirstOrDefault(p => p.IdLabel == labelId)?.Color ?? "white";
        return color;
    }

    private int GetCurrentProgress()
    {
        return MarkupData.CurrentProgress;
    }

    private string GetCurrentSqlDbName()
    {
        return MarkupData.NameDb;
    }

    private string GetNameFileEdit()
    {
        return MarkupData.NameImage;
    }

    private int GetIndexImage()
    {
        return MarkupData.CurrentIdImg;
    }


    private string GetActiveType()
    {
        return MarkupData.CurrentTypeLabel switch
        {
            TypeLabel.None => "-",
            TypeLabel.Box => "Box",
            TypeLabel.Polygon => "Polygon",
            TypeLabel.PolyLine => "Line",
            TypeLabel.Point => "Point",
            _ => string.Empty
        };
    }


}