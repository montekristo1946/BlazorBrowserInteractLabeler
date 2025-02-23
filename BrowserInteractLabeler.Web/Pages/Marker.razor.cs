using BrowserInteractLabeler.Web.Component.DrawingJobBox;
using BrowserInteractLabeler.Web.Infrastructure.Buffers;
using BrowserInteractLabeler.Web.Infrastructure.Handlers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;


namespace BrowserInteractLabeler.Web.Pages;

public class MarkerModel : ComponentBase
{
    [Inject] internal NavigationHandler _navigationHandler { get; set; }

    [Inject] internal KeyMapHandler _keyMapHandler { get; set; }

    [Inject] internal CacheModel _cacheModel { get; set; }

    [Inject] internal IJSRuntime _JSRuntime { get; set; }
    
    internal RenderFragment _tabBoxPanelTemplate{ get; set; } = null!;
    
    private TabBoxPanel? _tabBoxPanel = null;
    
    internal readonly string IdImagesPamel = "marker_panel";
    internal readonly KeyboardEventArgs _commandGoNext = new() { Key = "f", Code = "KeyF" };
    internal readonly KeyboardEventArgs _commandGoBack = new() { Key = "d", Code = "KeyD" };

    protected override void OnInitialized()
    {
        _tabBoxPanelTemplate  = CreateTabBoxPanelTemplate();
    }

    private RenderFragment CreateTabBoxPanelTemplate() => builder =>
    {
        builder.OpenComponent(0, typeof(TabBoxPanel));
        builder.AddAttribute(1,"RootWindowsSize",_cacheModel.RootWindowsSize);
        builder.AddAttribute(2,"AnnotationsOnPanel",_cacheModel.AnnotationsOnPanel);
        builder.AddAttribute(3,"LabelAll",_cacheModel.LabelAll);
        builder.AddAttribute(4,"ColorAll",_cacheModel.ColorAll);
        builder.AddAttribute(5,"ChoseActiveAnnotationIdAsync",_navigationHandler.SetActiveIdAnnotation);
        builder.AddAttribute(6,"ChoseHiddenLabelIdAsync",_navigationHandler.SetHiddenIdAnnotation);
        builder.AddAttribute(7,"HiddenAllLabelsAsync",_navigationHandler.HiddenAllLabels);
        builder.AddAttribute(8,"ChoseActiveLabelIdAsync",_navigationHandler.HandlerSetLabelId);
        
        builder.AddComponentReferenceCapture(9, value =>
        {
            _tabBoxPanel = value as TabBoxPanel
                           ?? throw new InvalidOperationException(
                               $"Не смог сконвертитировать {value.GetType()} в TabBoxPanel");
        });

        builder.CloseComponent();
   
    };


    internal async Task HandleKeyDown(KeyboardEventArgs arg)
    {
        await _keyMapHandler.HandleKeyDownAsync(arg);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_navigationHandler.SetMainFocusRootPanel)
        {
            await _JSRuntime.InvokeVoidAsync("FocusElement", IdImagesPamel);
            await _navigationHandler.CancelFocusRootPanelAsync();
        }
    }

    /// <summary>
    ///     Движение мыши
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    internal Task HandlerImagesPanelOnmouseupAsync(MouseEventArgs arg)
    {
        return Task.Run(() =>
        {
            const long button = 1;
            if (arg is { AltKey: true, Buttons: button })
            {

                _navigationHandler.HandlerDrawingPanelOnmousemoveAsync(arg);
            }
            else
            {
                _keyMapHandler.HandlerImagesPanelOnmouseupAsync(arg);
                _cacheModel.PointCursor = _navigationHandler.CalculateCursor(arg.OffsetX, arg.OffsetY);
            }
            
        });
    }
}