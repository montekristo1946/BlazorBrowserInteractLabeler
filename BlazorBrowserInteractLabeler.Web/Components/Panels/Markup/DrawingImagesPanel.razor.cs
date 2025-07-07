using System.Globalization;
using System.Reflection.Metadata;
using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BlazorBrowserInteractLabeler.Web.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Serilog;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Markup;

public partial class DrawingImagesPanel : ComponentBase
{
    [Inject] private MarkupData _markupData { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private Helper  _helper { get; set; } = null!;
    [Inject] private SvgConstructor  _svgConstructor { get; set; } = null!;

    private string WidthConvas => $"{(int)_markupData.SizeConvas.Width}px";
    private string HeightConvas => $"{(int)_markupData.SizeConvas.Height}px";

    [Parameter] public  Action<MouseEventArgs> HandlerOnmouseDown { get; set; }

    [Parameter] public Action<MouseEventArgs> HandlerOnMouseMove { get; set; }

    [Parameter] public Action<WheelEventArgs> HandleMouseWheel { get; set; }

    private RenderFragment CrosshairTemplate { get; set; } = null!;
    private Crosshair? _crosshairComponent = null;
    
    protected override void OnInitialized()
    {
        CrosshairTemplate = CreateCrosshairTemplate();
    }

    public async Task SetSizeConvas()
    {
        var width = _markupData.SizeConvas.Width;
        var height = _markupData.SizeConvas.Height;

        await JSRuntime.InvokeVoidAsync("SetBrowseSize", ConstantsArm.IdConvas,
            width,
            height);
    }

    public void OnUpdateImage()
    {
        InvokeAsync(async () => { await OnUpdateUiAsync(); });
    }



   


    private void MouseWheelHandler(WheelEventArgs args)
    {
        HandleMouseWheel?.Invoke(args);
    }


    private RenderFragment CreateCrosshairTemplate() => builder =>
    {
        builder.OpenComponent(0, typeof(Crosshair));
        
        builder.AddComponentReferenceCapture(1, value =>
        {
            _crosshairComponent = value as Crosshair
                                    ?? throw new InvalidOperationException(
                                        "Не смог сконвертитировать Crosshair в Crosshair");
        });

        builder.CloseComponent();
    };
    
    private string CssScale()
    {
        var scaleCurrent = _markupData.ScaleCurrent;
        var offsetX = _markupData.OffsetDrawImage.X;
        var offsetY = _markupData.OffsetDrawImage.Y;
        return $"transform: scale({scaleCurrent}) translate({offsetX}px, {offsetY}px)";
    }
    
    private async Task OnUpdateUiAsync()
    {
        var img = _markupData.ImagesUI;

        if (string.IsNullOrWhiteSpace(img))
            return;

        var sizeCash = _markupData.SizeConvas;
        var width = (int)sizeCash.Width;
        var height = (int)sizeCash.Height;
        await JSRuntime.InvokeVoidAsync("LoadImg", ConstantsArm.IdConvas, img, width, height);
    }

    private void MouseMoveHandler(MouseEventArgs args)
    {
        HandlerOnMouseMove?.Invoke(args);
        
        if(!_markupData.CrosshairData.IsShowCrosshair)
            return;

        var correctPoint = _helper.GetAbsoluteCoordinate(
            args.PageX,
            args.PageY, 
            _markupData.ImageMarkerPanelSize);
        
        var point = _helper.CorrectPoint(
            correctPoint,
            _markupData.ScaleCurrent,
            _markupData.OffsetDrawImage,
            _markupData.SizeConvas);

        _markupData.CrosshairData = _markupData.CrosshairData with
        {
            PointCursor = new PointT()
            {
                X = point.X,
                Y = point.Y
            },
            ScaleCurrent = _markupData.ScaleCurrent,
        };

        _crosshairComponent?.UpdateSvg(_markupData.CrosshairData);
    }

    private RenderFragment GetRenderAnnotation()=>
        async void (builder) =>
        {
            try
            {
                var figure = await _svgConstructor.CreateAnnotsFigure();
       
                builder.AddMarkupContent(0, figure);
            }
            catch (Exception e)
            {
                Log.Error("[GetRenderAnnotation] {@Exception}", e.Message);
            }
        };


  
}