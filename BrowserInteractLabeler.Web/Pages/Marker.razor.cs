using BrowserInteractLabeler.Common.DTO;
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
    
    internal RenderFragment TabBoxPanelTemplate{ get; set; } = null!;
    private TabBoxPanel? _tabBoxPanel = null;
    
    
    private RenderFragment CrosshairsTemplate { get; set; } = null!;
    private CrosshairModel? _crosshairModel = null;
    
    // private RenderFragment SvgPanelModelTemplate { get; set; } = null!;
    private SvgPanelModel? _svgPanelModel = null;
    
    internal RenderFragment ImagesPanelTemplate { get; set; } = null!;
    private ImagesPanel? _imagesPanel = null;
    
    internal RenderFragment ToolsPanelTemplate { get; set; } = null!;
    private ToolsPanel? _toolsPanel = null;
    
        
    internal readonly string IdImagesPamel = "marker_panel";
    internal readonly KeyboardEventArgs _commandGoNext = new() { Key = "f", Code = "KeyF" };
    internal readonly KeyboardEventArgs _commandGoBack = new() { Key = "d", Code = "KeyD" };

    protected override void OnInitialized()
    {
        TabBoxPanelTemplate  = CreateTabBoxPanelTemplate();
        
        CrosshairsTemplate = CreateCrosshairsTemplate();
        // SvgPanelModelTemplate = CreateSvgPanelModelTemplate();
        ImagesPanelTemplate = CreateImagesPanelTemplate();

        ToolsPanelTemplate = CreateToolsPanelTemplate();
        
        _navigationHandler.IsNewImageRendered = IsNewImageRendered;
        _navigationHandler.IsUpdatedUi = UpdateUi;

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_navigationHandler.SetMainFocusRootPanel)
        {
            await _JSRuntime.InvokeVoidAsync("FocusElement", IdImagesPamel);
            _navigationHandler.CancelFocusRootPanelAsync();
        }

        _imagesPanel?.OnUpdateImage();
    }

    private RenderFragment CreateToolsPanelTemplate() => builder =>
    {
        builder.OpenComponent(0, typeof(ToolsPanel));
        builder.AddAttribute(1,"ButtonDefaultMoveClick",() => _navigationHandler.ButtonDefaultPositionImg());
        builder.AddAttribute(2,"ButtonOnRectangleClick",() => _navigationHandler.EnableTypeLabel(TypeLabel.Box));
        builder.AddAttribute(3,"ButtonOnPolygonClick",() => _navigationHandler.EnableTypeLabel(TypeLabel.Polygon));
        builder.AddAttribute(4,"ButtonOnPolyLineClick",() => _navigationHandler.EnableTypeLabel(TypeLabel.PolyLine));
        builder.AddAttribute(5,"ButtonOnPointsClick",() => _navigationHandler.EnableTypeLabel(TypeLabel.Point));
        
        builder.AddComponentReferenceCapture(6, value =>
        {
            _toolsPanel = value as ToolsPanel
                                 ?? throw new InvalidOperationException(
                                     $"Не смог сконвертитировать {value.GetType()} в ToolsPanel");
        });

        builder.CloseComponent();
    };
    

    
    private RenderFragment CreateImagesPanelTemplate() => builder =>
    {
        builder.OpenComponent(0, typeof(ImagesPanel));
        builder.AddAttribute(1,"HandleMouse",_keyMapHandler.HandleImagePanelMouseAsync);
        builder.AddAttribute(2,"HandleRightClick",_keyMapHandler.HandleImagePanelMouseAsync);
        
        builder.AddAttribute(3,"HandlerOnmousedown",_keyMapHandler.HandlerImagesPanelOnmouseDownAsync);
        builder.AddAttribute(4,"HandlerOnmouseUp",_keyMapHandler.HandlerImagesPanelOnmouseUp);
        builder.AddAttribute(5,"HandlerOnmousemove",HandlerImagesPanelOnmouseupAsync);
        builder.AddAttribute(6,"HandleMouseWheel",_keyMapHandler.HandleWheelDrawingPanelMouseEventAsync);
        
        builder.AddAttribute(7,"SvgPanelTemplate",(RenderFragment)((svgPanelModel) =>
                {
                    svgPanelModel.OpenComponent(8, typeof(SvgPanelModel));
                    svgPanelModel.AddAttribute(9,"AnnotationsOnPanel",_cacheModel.AnnotationsOnPanel);
                    svgPanelModel.AddAttribute(10,"ScaleImg",_cacheModel.ScaleCurrent);
                    // svgPanelModel.AddAttribute(7,"CrosshairsTemplate",CrosshairsTemplate);
        
                    svgPanelModel.AddComponentReferenceCapture(3, value =>
                    {
                        _svgPanelModel = value as SvgPanelModel
                                         ?? throw new InvalidOperationException(
                                             $"Не смог сконвертитировать {value.GetType()} в SvgPanelModel");
                    });

                    svgPanelModel.CloseComponent();
                  
                }
            ));
     
        
        builder.AddComponentReferenceCapture(8, value =>
        {
            _imagesPanel = value as ImagesPanel
                             ?? throw new InvalidOperationException(
                                 $"Не смог сконвертитировать {value.GetType()} в ImagesPanel");
        });

        builder.CloseComponent();
    };
    
    // private RenderFragment CreateSvgPanelModelTemplate() => builder =>
    // {
    //     builder.OpenComponent(0, typeof(SvgPanelModel));
    //     builder.AddAttribute(1,"AnnotationsOnPanel",_cacheModel.AnnotationsOnPanel);
    //     builder.AddAttribute(2,"ScaleImg",_cacheModel.ScaleCurrent);
    //     builder.AddAttribute(3,"CrosshairsTemplate",CrosshairsTemplate);
    //     
    //     builder.AddComponentReferenceCapture(4, value =>
    //     {
    //         _svgPanelModel = value as SvgPanelModel
    //                           ?? throw new InvalidOperationException(
    //                               $"Не смог сконвертитировать {value.GetType()} в SvgPanelModel");
    //     });
    //
    //     builder.CloseComponent();
    // };
    //
    
    
    private RenderFragment CreateCrosshairsTemplate() => builder =>
    {
        builder.OpenComponent(0, typeof(CrosshairModel));
        builder.AddAttribute(1,"PointCursor",_cacheModel.PointCursor);
        builder.AddAttribute(2,"ScaleCurrent",_cacheModel.ScaleCurrent);
        builder.AddAttribute(3,"ActiveTypeLabel",_cacheModel.ActiveTypeLabel);
        builder.AddAttribute(4,"PointCursor",_cacheModel.ActiveLabelColor);
        builder.AddAttribute(5,"AnnotationsOnPanel",_cacheModel.AnnotationsOnPanel);
        
        builder.AddComponentReferenceCapture(6, value =>
        {
            _crosshairModel = value as CrosshairModel
                             ?? throw new InvalidOperationException(
                                 $"Не смог сконвертитировать {value.GetType()} в CrosshairModel");
        });

        builder.CloseComponent();
    };

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
    
    private void UpdateUi()
    {
        StateHasChanged();
    }


    private void IsNewImageRendered()
    {
         _tabBoxPanel?.IsNewImageRendered();
         StateHasChanged();
    }

   


    internal async Task HandleKeyDown(KeyboardEventArgs arg)
    {
        await _keyMapHandler.HandleKeyDownAsync(arg);
    }



    /// <summary>
    ///     Движение мыши
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    internal void HandlerImagesPanelOnmouseupAsync(MouseEventArgs arg)
    {
        const long button = 1;
        
         Task.Run(() =>
        {
            
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
        
        // await InvokeAsync(StateHasChanged);
    }
}