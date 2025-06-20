using BlazorBrowserInteractLabeler.ARM;
using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BlazorBrowserInteractLabeler.Web.Common;
using BlazorBrowserInteractLabeler.Web.Components.Panels.Markup;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorBrowserInteractLabeler.Web.Components.Pages;

public partial class ImageMarker : ComponentBase, IDisposable
{
    
    [Inject] private KeyMapHandler KeyMapHandler { get; set; } = null!;
    [Inject] private MarkupData MarkupData { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    private RenderFragment ImagesPanelTemplate { get; set; } = null!;
    private DrawingImagesPanel? _imagesPanelComponent = null;

    protected override void OnInitialized()
    {
        ImagesPanelTemplate = CreateImagesPanelTemplate();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("window.registerViewportChangeCallback",
                DotNetObjectReference.Create(this));
            OnResize(-1,-1);


        }
    }

    private void UpdateUi()
    {
        InvokeAsync(StateHasChanged);
        _imagesPanelComponent?.OnUpdateImage();
    }

    private RenderFragment CreateImagesPanelTemplate() => builder =>
    {
        builder.OpenComponent(0, typeof(DrawingImagesPanel));
        builder.AddAttribute(1,nameof(DrawingImagesPanel.HandlerOnmouseDown),KeyMapHandler.HandlerOnMouseDown);
        builder.AddAttribute(2,nameof(DrawingImagesPanel.HandlerOnMouseMove),KeyMapHandler.HandlerOnMouseMove);
        builder.AddAttribute(3,nameof(DrawingImagesPanel.HandleMouseWheel),KeyMapHandler.HandleMouseWheel);
        
        builder.AddComponentReferenceCapture(4, value =>
        {
            _imagesPanelComponent = value as DrawingImagesPanel
                                    ?? throw new InvalidOperationException(
                                        "Не смог сконвертитировать ImagesPanel в ImagesPanel");
        });

        builder.CloseComponent();
    };

    public void Dispose()
    {
     
    }
    
    [JSInvokable]
    public void OnResize(int width, int height)
    {
        InvokeAsync(async () =>
        {
            var sizeBrowse = await JSRuntime.InvokeAsync<SizeWindows>("GetBrowseSize", ConstantsArm.DrawingImagesPanelRoot);

            MarkupData.ImageMarkerPanelSize = sizeBrowse;
            
            await _imagesPanelComponent?.SetSizeConvas()!;
            UpdateUi();
        });
    }
    
}