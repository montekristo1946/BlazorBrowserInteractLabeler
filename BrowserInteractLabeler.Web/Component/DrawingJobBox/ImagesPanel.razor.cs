using BrowserInteractLabeler.Repository;
using BrowserInteractLabeler.Web.Infrastructure.Buffers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Web.Component.DrawingJobBox;

public class 
    ImagesPanelModel : ComponentBase
{
    internal readonly string IdCanvas = "chartCanvas_1";
    internal string CssScale => _cacheModel.CssScale;// "transform: scale(1.0) translate(+0%, -0%);";
    internal string WidthImg => $"{(int)_cacheModel.SizeDrawImage.Width}px";
    internal string HeightImg => $"{(int)_cacheModel.SizeDrawImage.Height}px";

    internal string WidthMainWin => $"{(int)_cacheModel.ImageWindowsSize.Width}px";

    internal string HeightMainWin => $"{(int)_cacheModel.ImageWindowsSize.Height}px";

    [Parameter] public EventCallback<MouseEventArgs> HandleMouse { get; set; }

    [Parameter] public EventCallback<MouseEventArgs> HandleRightClick { get; set; }

    [Parameter] public EventCallback<MouseEventArgs> HandlerOnmousedown { get; set; }

    [Parameter] public EventCallback<MouseEventArgs> HandlerOnmouseUp { get; set; }

    [Parameter] public EventCallback<MouseEventArgs> HandlerOnmousemove { get; set; }

    [Parameter] public RenderFragment SvgPanelTemplate { get; set; }

    [Parameter] public EventCallback<WheelEventArgs> HandleMouseWheel { get; set; }

    [Inject] internal IJSRuntime _JSRuntime { get; set; }

    private readonly ILogger _logger = Log.ForContext<MockupRepository>();

    [Inject] internal CacheModel _cacheModel { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadImageJSAsync();
        }
    }

    private string Temp = string.Empty;
    internal async Task LoadImageJSAsync()
    {
        // var imagesSize = _cacheModel.SizeDrawImage;
        var width = (int)_cacheModel.SizeDrawImage.Width;
        var height = (int)_cacheModel.SizeDrawImage.Height;
        await _JSRuntime.InvokeVoidAsync("LoadImg", IdCanvas, _cacheModel.ImagesBase64, width, height);
        // _logger.Debug("button_testClick");
        // StateHasChanged();
    }
}