using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Component.DrawingJobBox;
using BrowserInteractLabeler.Infrastructure.Configs;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Infrastructure;

public class KeyMapHandler
{
    private readonly NavigationHandler _navigationHandler;

    private readonly ServiceConfigs _serviceConfigs;
    private readonly ILogger _logger = Log.ForContext<NavigationHandler>();
    private Dictionary<string, string> mLookup;

    public KeyMapHandler(NavigationHandler navigationHandler, ServiceConfigs serviceConfigs)
    {
        _navigationHandler = navigationHandler ?? throw new ArgumentNullException(nameof(navigationHandler));
        _serviceConfigs = serviceConfigs ?? throw new ArgumentNullException(nameof(serviceConfigs));
    }


    /// <summary>
    ///     ButtonGoNextClick on ButtonGoBackClick
    /// </summary>
    /// <param name="arg"></param>
    public async Task HandleKeyDownAsync(KeyboardEventArgs arg)
    {
        // _logger.Debug("[KeyMapHandler:HandleKeyDownAsync] key down {@KeyboardEventArgs}", arg);

        if (arg.Repeat)
            return;

        await BasicFunctions(arg.Code);
        await MarkupFunctions(arg.Code, _serviceConfigs.Colors);
    }

    private async Task MarkupFunctions(string codeKey, ColorModel[] serviceConfigsColors)
    {
        var findKey = serviceConfigsColors.FirstOrDefault(p => p.KeyCode == codeKey);
        if (findKey is null)
            return;

        await _navigationHandler.HandlerSetLabelIdAsync(findKey.IdLabel);
    }

    private async Task BasicFunctions(string codeKey)
    {
        switch (codeKey)
        {
            case "KeyD":
            case "ArrowLeft":
            {
                await _navigationHandler.ButtonGoBackClick();
                break;
            }
            case "KeyF":
            case "ArrowRight":
            {
                await _navigationHandler.ButtonGoNextClick();
                break;
            }
            case "Delete":
            case "KeyX":
            {
                await _navigationHandler.DeleteAnnotation();
                break;
            }
            case "KeyE":
            {
                 _navigationHandler.EventEditAnnot();
                break;
            }
            case "KeyQ":
            {
                await _navigationHandler.EnableTypeLabel(TypeLabel.Box);
                break;
            }
            case "KeyW":
            {
                await _navigationHandler.EnableTypeLabel(TypeLabel.Polygon);
                break;
            }
            case "KeyA":
            {
                await _navigationHandler.EnableTypeLabel(TypeLabel.PolyLine);
                break;
            }
            case "KeyS":
            {
                await _navigationHandler.EnableTypeLabel(TypeLabel.Point);
                break;
            }
            case "Space": //Sapase
            {
                await _navigationHandler.ButtonDefaultPositionImg();
                break;
            }
        }
    }


    /// <summary>
    ///     Click markup
    /// </summary>
    /// <param name="arg"></param>
    public async Task HandleImagePanelMouseAsync(MouseEventArgs arg)
    {
        if (arg.AltKey)
            return;

        const int leftButton = 0;
        const int rightButton = 2;

        switch (arg.Button)
        {
            case leftButton:
                await _navigationHandler.HandleImagePanelMouseAsync(arg, DateTime.Now);
                break;
            case rightButton:
                await _navigationHandler.HandleImagePanelMouseRightButtonAsync(arg, DateTime.Now);
                break;
        }
    }

    /// <summary>
    ///     Левую клавишу мыши нажали
    /// </summary>
    /// <param name="arg"></param>
    public async Task HandlerImagesPanelOnmouseDownAsync(MouseEventArgs arg)
    {
        if (arg.AltKey)
        {
            await _navigationHandler.HandlerImagesPanelOnmousedownAsync(arg,
                DateTime.Now); //кооректируе точку отчета при перемещении изображения (первое нажатие на мышь)
            return;
        }
        _navigationHandler.ResetSelectPointAsync();
        await _navigationHandler.HandlerSelectPointAsync(arg,
            DateTime.Now); //кооректируе точку отчета при перемещении изображения (первое нажатие на мышь)
    }



    /// <summary>
    ///     Общая панель для отрисовки перемещение мыши
    /// </summary>
    /// <param name="arg"></param>
    public async Task HandlerDrawingPanelOnmousemoveAsync(MouseEventArgs arg)
    {
        const long button = 1;
        if (arg is { AltKey: true, Buttons: button })
        {
            await _navigationHandler.HandlerDrawingPanelOnmousemoveAsync(arg, DateTime.Now);
        }
    }

    /// <summary>
    ///     Перемещение точки
    /// </summary>
    /// <param name="args"></param>
    /// <param name="svgPanelRef"></param>
    /// <param name="arg"></param>
    public void HandlerImagesPanelOnmouseupAsync(MouseEventArgs args)
    {
        // _logger.Debug("[HandlerImagesPanelOnmouseupAsync] {@MouseEventArgs}",args);
        const long buttons = 1;
        if (args is { AltKey: false, Buttons: buttons })
            _navigationHandler.HandlerMovePointAsync(args, DateTime.Now);
    }


    /// <summary>
    ///     Общая панель для отрисовки , колесо мыши
    /// </summary>
    /// <param name="arg"></param>
    public async Task HandleWheelDrawingPanelMouseEventAsync(WheelEventArgs arg)
    {
        await _navigationHandler.WheelDrawingPanelMouseEventAsync(arg, DateTime.Now);
    }
}