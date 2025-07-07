using BlazorBrowserInteractLabeler.Web.Components.Panels.PagesSelector;
using BlazorBrowserInteractLabeler.Web.Components.Panels.Settings;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Pages;

public partial class SettingsPage : ComponentBase
{
    
    private RenderFragment PagesSelectorTemplate { get; set; } = null!;
    private PagesSelectorComponent? _pagesSelector = null;
    
    private RenderFragment SettingComponentTemplate { get; set; } = null!;
    private SettingComponent? _settingComponent = null;
    protected override void OnInitialized()
    {
        PagesSelectorTemplate = CreatePagesSelectorTemplate();
        SettingComponentTemplate = SettingComponentTemplateTemplate();
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
    
    private RenderFragment SettingComponentTemplateTemplate() => builder =>
    {
        builder.OpenComponent(0, typeof(SettingComponent));
        
        builder.AddComponentReferenceCapture(1, value =>
        {
            _settingComponent = value as SettingComponent
                                ?? throw new InvalidOperationException(
                                    "Не смог сконвертитировать SettingComponent в SettingComponent");
        });

        builder.CloseComponent();
    };
}