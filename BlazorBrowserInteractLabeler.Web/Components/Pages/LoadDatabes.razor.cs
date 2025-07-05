using BlazorBrowserInteractLabeler.Web.Components.Panels.Navigation;
using BlazorBrowserInteractLabeler.Web.Components.Panels.PagesSelector;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Pages;

public partial class LoadDatabes : ComponentBase
{
    
    private RenderFragment PagesSelectorTemplate { get; set; } = null!;
    private PagesSelectorComponent? _pagesSelector = null;
    
    protected override void OnInitialized()
    {
        PagesSelectorTemplate = CreatePagesSelectorTemplate();
    }
    
    private RenderFragment CreatePagesSelectorTemplate() => builder =>
    {
        builder.OpenComponent(0, typeof(PagesSelectorComponent));
        
        builder.AddComponentReferenceCapture(1, value =>
        {
            _pagesSelector = value as PagesSelectorComponent
                             ?? throw new InvalidOperationException(
                                 "Не смог сконвертитировать PagesSelectorComponent в PagesSelectorComponent");
        });

        builder.CloseComponent();
    };
    
}