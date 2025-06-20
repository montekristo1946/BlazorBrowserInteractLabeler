using BrowserInteractLabeler.Common.DTO;
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
    internal string CssScale => _cacheModel.CssScale; // "transform: scale(1.0) translate(+0%, -0%);";
    internal string WidthImg => $"{(int)_cacheModel.SizeDrawImage.Width}px";
    internal string HeightImg => $"{(int)_cacheModel.SizeDrawImage.Height}px";

    // internal string WidthMainWin => $"{(int)_cacheModel.ImageWindowsSize.Width}px";
    //
    // internal string HeightMainWin => $"{(int)_cacheModel.ImageWindowsSize.Height}px";

    [Parameter] public Action<MouseEventArgs> HandleMouse { get; set; }

    [Parameter] public Action<MouseEventArgs> HandleRightClick { get; set; }

    [Parameter] public Action<MouseEventArgs> HandlerOnmousedown { get; set; }

    [Parameter] public Action<MouseEventArgs> HandlerOnmouseUp { get; set; }

    [Parameter] public Action<MouseEventArgs> HandlerOnmousemove { get; set; }
    
    [Parameter] public Action<WheelEventArgs> HandleMouseWheel { get; set; }
    
    [Parameter] public RenderFragment SvgPanelTemplate { get; set; }

    [Inject] internal IJSRuntime JSRuntime { get; set; } = null!;


    [Inject] internal CacheModel _cacheModel { get; set; }

    internal readonly string IdRootDiv = "ImagesPanelModelRootDiv";
    internal readonly string IdCanvas = "CanvasImagesPanelModel";
    internal readonly string ScaleDiv = "ScaleDivImagesPanelModel";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("window.registerViewportChangeCallback",
                DotNetObjectReference.Create(this));

            OnResize(-1, -1);
        }
    }

    [JSInvokable]
    public void OnResize(int width, int height)
    {
        var tolerance = 0.01;
        InvokeAsync(async () =>
        {
            var sizeBrowse = await JSRuntime.InvokeAsync<SizeF>("GetBrowseSize", IdRootDiv);

            var newWidth = sizeBrowse.Width;
            var newHeight = sizeBrowse.Height;

            if (Math.Abs(newHeight - _cacheModel.SizeDrawImage.Height) < tolerance &&
                Math.Abs(newWidth - _cacheModel.SizeDrawImage.Width) < tolerance)
                return;

            _cacheModel.SizeDrawImage = new SizeF()
            {
                Height = newHeight,
                Width = newWidth
            };
   
        

            await JSRuntime.InvokeVoidAsync("SetBrowseSize", IdCanvas,
                newWidth,
                newHeight);

            await JSRuntime.InvokeVoidAsync("SetBrowseSize", ScaleDiv,
                newWidth,
                newHeight);
            
      
        });
    }

    internal void OnUpdateImage()
    {
        InvokeAsync(async () => { await OnUpdateUiAsync(); });
    }
    
    private async Task OnUpdateUiAsync()
    {
        var img = _cacheModel.ImagesBase64;
        
        if(string.IsNullOrWhiteSpace(img))
            return;
        
        var width = (int)_cacheModel.SizeDrawImage.Width;
        var height = (int)_cacheModel.SizeDrawImage.Height;
        await JSRuntime.InvokeVoidAsync("LoadImg", IdCanvas, _cacheModel.ImagesBase64, width, height);
        
     
    }
}