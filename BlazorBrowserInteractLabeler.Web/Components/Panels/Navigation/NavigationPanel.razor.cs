using BlazorBrowserInteractLabeler.ARM.Handlers;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Navigation;

public partial class NavigationPanel : ComponentBase
{
    [Inject] private IMediator _mediator { get; set; } = null!;
    [Inject] private SettingsData _settingsData { get; set; } = null!;
    [Inject] private MarkupData _markupData { get; set; } = null!;

    [Parameter] public Action IsNeedUpdateUI { get; set; }


    private async Task OnClickBackImg()
    {
        await _mediator.Send(new LoadNextImageQueries() { IsForward = false });
        IsNeedUpdateUI?.Invoke();
    }

    private async Task OnClickNextImg()
    {
        await _mediator.Send(new LoadNextImageQueries() { IsForward = true });
        IsNeedUpdateUI?.Invoke();
    }

    private  async Task OnClickSave()
    {
        await _mediator.Send(new SaveAnnotationsOnSlowStorageQueries());
        IsNeedUpdateUI?.Invoke();
    }

    private void OnClickUndo()
    {
        IsNeedUpdateUI?.Invoke();
    }

    private void OnClickRedo()
    {
        IsNeedUpdateUI?.Invoke();
    }

    private async Task OnClickInitRectangle()
    {
        await _mediator.Send(new InitNewAnnotQueries() { TypeLabel = TypeLabel.Box });
        IsNeedUpdateUI?.Invoke();
    }

    private async Task OnClickInitPolygon()
    {
        await _mediator.Send(new InitNewAnnotQueries() { TypeLabel = TypeLabel.Polygon });
        IsNeedUpdateUI?.Invoke();
    }

    private async Task  OnClickInitPolyline()
    {
        await _mediator.Send(new InitNewAnnotQueries() { TypeLabel = TypeLabel.PolyLine });
        IsNeedUpdateUI?.Invoke();
    }

    private async Task OnClickInitPoints()
    {
        await _mediator.Send(new InitNewAnnotQueries() { TypeLabel = TypeLabel.Point });
        IsNeedUpdateUI?.Invoke();
    }
    private async Task ClickSetupIndexImage(ChangeEventArgs changeEventArgs)
    {
        var resultTryParse = Int32.TryParse((string?)changeEventArgs.Value, out var indexImg);
        if (!resultTryParse)
            return;
        
        await _mediator.Send(new LoadByIndexImageQueries() { IndexImage = indexImg });
        IsNeedUpdateUI?.Invoke();
    }
    
    private async Task OnClickRestorePositionImage()
    {
        await _mediator.Send(new RestorePositionImageQueries() );
        IsNeedUpdateUI?.Invoke();
    }

    private string GetBacgroundLabel()
    {
        var labelId = _markupData.CurrentLabelId;
        var colorModels = _settingsData.ColorModel;
        var color = colorModels.FirstOrDefault(p => p.IdLabel == labelId)?.Color ?? "white";
        return color;
    }

    private int GetCurrentProgress()
    {
        return _markupData.CurrentProgress;
    }

    private string GetCurrentSqlDbName()
    {
        return _markupData.NameDb;
    }

    private string GetNameFileEdit()
    {
        return _markupData.NameImage;
    }

    private int GetIndexImage()
    {
        return _markupData.CurrentIdImg;
    }


    private string GetActiveType()
    {
        return _markupData.CurrentTypeLabel switch
        {
            TypeLabel.None => "Non",
            TypeLabel.Box => "Box",
            TypeLabel.Polygon => "Poligon",
            TypeLabel.PolyLine => "Line",
            TypeLabel.Point => "Point",
            _ => string.Empty
        };
    }
}