using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BlazorBrowserInteractLabeler.Web.Common;
using BlazorBrowserInteractLabeler.Web.Components.Panels.Labeling;
using BlazorBrowserInteractLabeler.Web.Components.Panels.Markup;
using BlazorBrowserInteractLabeler.Web.Components.Panels.Navigation;
using BlazorBrowserInteractLabeler.Web.Components.Panels.PagesSelector;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorBrowserInteractLabeler.Web.Components.Pages;

public partial class ImageMarker : ComponentBase, IDisposable
{

    [Inject] private KeyMapHandler KeyMapHandler { get; set; } = null!;
    [Inject] private MarkupData MarkupData { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    [Inject] private ProjectsLocalHandler _projectsLocalHandler { get; set; } = null!;

    [Inject] private IMediator _mediator { get; set; } = null!;
    private RenderFragment DrawingImagesPanelTemplate { get; set; } = null!;
    private DrawingImagesPanel? _drawingImagesPanelComponent = null;

    private RenderFragment NavigationPanelTemplate { get; set; } = null!;
    private NavigationPanel? _navigationPanel = null;

    private RenderFragment LabelingPanelTemplate { get; set; } = null!;
    private LabelingPanel? _labelingPanelComponent = null;

    private RenderFragment PagesSelectorTemplate { get; set; } = null!;
    private PagesSelectorComponent? _pagesSelector = null;
    protected override async Task OnInitializedAsync()
    {
        await _mediator.Send(new LoadConfigurationQueries());
        DrawingImagesPanelTemplate = CreateDrawingImagesPanelTemplate();
        NavigationPanelTemplate = CreateNavigationPanelTemplate();
        LabelingPanelTemplate = CreateLabelingPanelTemplate();
        PagesSelectorTemplate = CreatePagesSelectorTemplate();

        KeyMapHandler.IsNeedUpdateUi += UpdateUi;
    }




    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("window.registerViewportChangeCallback", DotNetObjectReference.Create(this));
            OnResize(-1, -1);
            await JSRuntime.InvokeVoidAsync("FocusElement", ConstantsArm.ImageMarkerRoot);

        }
    }

    private void UpdateUi()
    {
        _drawingImagesPanelComponent?.OnUpdateImage();
        _labelingPanelComponent?.UpdateUi();
    }

    private void UpdateUiNavigation()
    {
        _drawingImagesPanelComponent?.OnUpdateImage();
        _labelingPanelComponent?.UpdateUi();
        UpdateUiLabelingPanel();
    }

    private void UpdateUiLabelingPanel()
    {
        StateHasChanged();
    }
    private RenderFragment CreateDrawingImagesPanelTemplate() => builder =>
    {
        builder.OpenComponent(0, typeof(DrawingImagesPanel));
        builder.AddAttribute(1, nameof(DrawingImagesPanel.HandlerOnmouseDown), ClickMouseDown);
        builder.AddAttribute(1, nameof(DrawingImagesPanel.HandlerOnmouseUp), ClickMouseUp);
        builder.AddAttribute(2, nameof(DrawingImagesPanel.HandlerOnMouseMove), MouseMove);
        builder.AddAttribute(3, nameof(DrawingImagesPanel.HandleMouseWheel), HandleMouseWheel);

        builder.AddComponentReferenceCapture(4, value =>
        {
            _drawingImagesPanelComponent = value as DrawingImagesPanel
                                    ?? throw new InvalidOperationException(
                                        "Не смог сконвертитировать ImagesPanel в ImagesPanel");
        });

        builder.CloseComponent();
    };

    private RenderFragment CreateNavigationPanelTemplate() => builder =>
    {
        builder.OpenComponent(0, typeof(NavigationPanel));
        builder.AddAttribute(1, nameof(NavigationPanel.IsNeedUpdateUi), UpdateUiNavigation);
        builder.AddComponentReferenceCapture(2, value =>
        {
            _navigationPanel = value as NavigationPanel
                               ?? throw new InvalidOperationException(
                                   "Не смог сконвертитировать NavigationPanel в NavigationPanel");
        });

        builder.CloseComponent();
    };
    private RenderFragment CreateLabelingPanelTemplate() => builder =>
    {
        builder.OpenComponent(0, typeof(LabelingPanel));
        builder.AddAttribute(1, nameof(LabelingPanel.IsUpdateMenu), UpdateUiLabelingPanel);
        builder.AddComponentReferenceCapture(1, value =>
        {
            _labelingPanelComponent = value as LabelingPanel
                                      ?? throw new InvalidOperationException(
                                          "Не смог сконвертитировать NavigationPanel в LabelingPanel");
        });

        builder.CloseComponent();
    };

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

    private void HandleMouseWheel(WheelEventArgs args)
    {
        var isUpdate = KeyMapHandler.HandleMouseWheel(args);

        if (isUpdate)
        {
            _drawingImagesPanelComponent?.ResetCrossHair();
            UpdateUiLabelingPanel();
        }
    }


    private void ClickMouseUp(MouseEventArgs args)
    {
        InvokeAsync(async () =>
        {
            await KeyMapHandler.HandlerOnMouseUp(args);
            UpdateUiLabelingPanel();
        });
    }


    private void ClickMouseDown(MouseEventArgs args)
    {
        InvokeAsync(async () =>
        {
            await KeyMapHandler.HandlerOnMouseDown(args);
            _labelingPanelComponent?.UpdateUi();
        });
    }

    private void MouseMove(MouseEventArgs args)
    {
        InvokeAsync(async () =>
        {
            await KeyMapHandler.HandlerOnMouseMove(args);
        });
    }








    [JSInvokable]
    public void OnResize(int width, int height)
    {
        InvokeAsync(async () =>
        {
            var sizeBrowse = await JSRuntime.InvokeAsync<SizeWindows>("GetBrowseSize", ConstantsArm.DrawingImagesPanelRoot);

            MarkupData.ImageMarkerPanelSize = sizeBrowse;

            await _drawingImagesPanelComponent?.SetSizeConvas()!;
            await _mediator.Send(new RestorePositionImageQueries());
            UpdateUi();
        });
    }



    private async Task HandleKeyDown(KeyboardEventArgs arg)
    {
        await KeyMapHandler.HandleKeyDownAsync(arg);
    }

    public void Dispose()
    {
        if (KeyMapHandler?.IsNeedUpdateUi is null)
            return;

        KeyMapHandler.IsNeedUpdateUi -= UpdateUi;
    }


}