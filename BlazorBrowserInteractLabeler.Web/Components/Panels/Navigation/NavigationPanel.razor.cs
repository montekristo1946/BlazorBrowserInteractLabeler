using BlazorBrowserInteractLabeler.ARM.Handlers;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Navigation;

public partial class NavigationPanel : ComponentBase
{
  
    [Inject] private IMediator _mediator { get; set; } = null!;
    [Parameter]  public Action IsNeedUpdateUI { get; set; }
    

    private  async Task  OnClickBackImg()
    {
        await _mediator.Send(new LoadNextImageQueries(){IsForward = false});
        IsNeedUpdateUI?.Invoke();
    }

    private async Task OnClickNextImg()
    {
        await _mediator.Send(new LoadNextImageQueries(){IsForward = true});
        IsNeedUpdateUI?.Invoke();
    }

    private void OnClickSave()
    {
    }

    private void OnClickUndo()
    {
    }

    private void OnClickRedo()
    {
    }

    private async Task  OnClickInitRectangle()
    {
        await _mediator.Send(new InitNewAnnotQueries(){TypeLabel = TypeLabel.Box});
    }

    private void OnClickInitPolygon()
    {
    }

    private void OnClickInitPolyline()
    {
    }

    private void OnClickInitPoints()
    {
    }

    private string GetBacgroundLabel()
    {
        return "red";
    }

    private int GetCurrentProgress()
    {
        return 110;
    }

    private string GetCurrentSqlDbName()
    {
        return "000_2025-06-19_08-11-35-224402";
    }

    private string GetNameFileEdit()
    {
        return "Looong_nane_12345678965432134654321354sdfsd54fsdf ";
    }
}