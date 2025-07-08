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
    [Inject] private MarkupData MarkupData { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private Helper  Helper { get; set; } = null!;
    [Inject] private SvgConstructor  SvgConstructor { get; set; } = null!;



    [Parameter] public  Action<MouseEventArgs> HandlerOnmouseDown { get; set; } = null!;

    [Parameter] public Action<MouseEventArgs> HandlerOnMouseMove { get; set; } = null!;

    [Parameter] public Action<WheelEventArgs> HandleMouseWheel { get; set; } = null!;
    
    [Parameter] public  Action<MouseEventArgs> HandlerOnmouseUp { get; set; } = null!;

    private RenderFragment CrosshairTemplate { get; set; } = null!;
    private Crosshair? _crosshairComponent = null;
    private string WidthConvas => $"{(int)MarkupData.SizeConvas.Width}px";
    private string HeightConvas => $"{(int)MarkupData.SizeConvas.Height}px";

    private const string ColorError = "red";
    protected override void OnInitialized()
    {
        CrosshairTemplate = CreateCrosshairTemplate();
    }

    public async Task SetSizeConvas()
    {
        var width = MarkupData.SizeConvas.Width;
        var height = MarkupData.SizeConvas.Height;

        await JsRuntime.InvokeVoidAsync("SetBrowseSize", ConstantsArm.IdConvas,
            width,
            height);
    }

    public void OnUpdateImage()
    {
        InvokeAsync(async () => { await OnUpdateUiAsync(); });
    }


    public void ResetCrossHair()
    {
        var emptyCrosshair = new CrosshairData()
        {
            Color = "",
            PointCursor = new PointT(),
            ScaleCurrent = 0,
            IsShowCrosshair = false
        };
        _crosshairComponent?.UpdateSvg(emptyCrosshair);
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
        var scaleCurrent = MarkupData.ScaleCurrent;
        var offsetX = MarkupData.OffsetDrawImage.X;
        var offsetY = MarkupData.OffsetDrawImage.Y;
        return $"transform: scale({scaleCurrent}) translate({offsetX}px, {offsetY}px)";
    }
    
    private async Task OnUpdateUiAsync()
    {
        var img = MarkupData.ImagesUI;

        if (string.IsNullOrWhiteSpace(img))
            return;

        var sizeCash = MarkupData.SizeConvas;
        var width = (int)sizeCash.Width;
        var height = (int)sizeCash.Height;
        await JsRuntime.InvokeVoidAsync("LoadImg", ConstantsArm.IdConvas, img, width, height);
    }

    private void MouseMoveHandler(MouseEventArgs args)
    {
        HandlerOnMouseMove?.Invoke(args);
        
        if(!MarkupData.CrosshairData.IsShowCrosshair)
            return;

        var correctPoint = Helper.GetAbsoluteCoordinate(
            args.PageX,
            args.PageY, 
            MarkupData.ImageMarkerPanelSize);
        
        var point = Helper.CorrectPoint(
            correctPoint,
            MarkupData.ScaleCurrent,
            MarkupData.OffsetDrawImage,
            MarkupData.SizeConvas);

        MarkupData.CrosshairData = MarkupData.CrosshairData with
        {
            PointCursor = new PointT()
            {
                X = point.X,
                Y = point.Y
            },
            ScaleCurrent = MarkupData.ScaleCurrent,
        };

        _crosshairComponent?.UpdateSvg(MarkupData.CrosshairData);
    }

    private RenderFragment GetRenderAnnotation()=>
        async void (builder) =>
        {
            try
            {
                var figure = await SvgConstructor.CreateAnnotsFigure();
       
                builder.AddMarkupContent(0, figure);
            }
            catch (Exception e)
            {
                Log.Error("[GetRenderAnnotation] {@Exception}", e.Message);
            }
        };


    private RenderFragment GetRenderTextHelper() => (builder) =>
    {
        if(string.IsNullOrWhiteSpace(MarkupData.ErrorMessage))
            return;
        
        var figure = SvgConstructor.CreateTextHelper(ColorError, [MarkupData.ErrorMessage]);
        builder.AddMarkupContent(0, figure);
    };
}