using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.Web.Components.Panels.LoaderDB;
using BlazorBrowserInteractLabeler.Web.Components.Panels.Navigation;
using BlazorBrowserInteractLabeler.Web.Components.Panels.PagesSelector;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Pages;

public partial class SelectorWorks : ComponentBase
{
    [Inject] private IMediator Mediator { get; set; } = null!;
    private RenderFragment PagesSelectorTemplate { get; set; } = null!;
    private PagesSelectorComponent? _pagesSelector = null;
    
    private RenderFragment WorksShowerComponentTemplate { get; set; } = null!;
    private WorksShowerComponent? _worksShowerComponent = null;
    
    
    protected override async Task OnInitializedAsync()
    {
        await Mediator.Send(new LoadConfigurationQueries());
        PagesSelectorTemplate = CreatePagesSelectorTemplate();
        WorksShowerComponentTemplate = CreateWorksShowerComponentTemplate();
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
    
    private RenderFragment CreateWorksShowerComponentTemplate() => builder =>
    {
        builder.OpenComponent(0, typeof(WorksShowerComponent));
        
        builder.AddComponentReferenceCapture(1, value =>
        {
            _worksShowerComponent = value as WorksShowerComponent
                                    ?? throw new InvalidOperationException(
                                        "Не смог сконвертитировать WorksShowerComponent в WorksShowerComponent");
        });

        builder.CloseComponent();
    };
    
}