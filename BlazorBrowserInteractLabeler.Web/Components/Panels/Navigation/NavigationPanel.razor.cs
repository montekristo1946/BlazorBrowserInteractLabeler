using BlazorBrowserInteractLabeler.ARM.Handlers;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Navigation;

public partial class NavigationPanel : ComponentBase
{
    
    [Parameter]  public Action ClickBackImages { get; set; }
    
    [Parameter]  public Action ClickNextImages { get; set; }

    private void OnClickBackImg()
    {
        ClickBackImages?.Invoke();
    }

    private void OnClickNextImg()
    {
        ClickNextImages?.Invoke();
    }
}