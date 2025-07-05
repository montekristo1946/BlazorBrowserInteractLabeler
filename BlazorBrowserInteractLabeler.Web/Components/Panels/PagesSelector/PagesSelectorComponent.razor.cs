using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.PagesSelector;

public partial class PagesSelectorComponent : ComponentBase
{
    [Inject] private NavigationManager NavManager { get; set; }
    private void ClickPageSettings()
    {
        NavManager.NavigateTo("SettingsPage");
    }

    private void ClickPageMarker()
    {
        NavManager.NavigateTo("ImageMarker");

    }

    private void ClickPageLoadDatabes()
    {
        NavManager.NavigateTo("LoadDatabes");

    }
}