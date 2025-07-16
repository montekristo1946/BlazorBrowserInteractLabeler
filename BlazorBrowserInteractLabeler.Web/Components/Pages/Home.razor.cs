using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Pages;

public partial class Home: ComponentBase
{
    [Inject] private NavigationManager NavManager { get; set; } = null!;
    [Inject] private IMediator Mediator { get; set; } = null!;
    
    protected override async Task OnInitializedAsync()
    {
        await Mediator.Send(new UnloadDataBaseQueries());
        NavManager.NavigateTo("SelectorWorks");
    }
}