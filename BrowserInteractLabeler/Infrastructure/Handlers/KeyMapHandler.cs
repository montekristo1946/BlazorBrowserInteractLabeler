using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
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

    public KeyMapHandler(NavigationHandler navigationHandler,ServiceConfigs serviceConfigs)
    {
        _navigationHandler = navigationHandler ?? throw new ArgumentNullException(nameof(navigationHandler));
        _serviceConfigs = serviceConfigs ?? throw new ArgumentNullException(nameof(serviceConfigs));
    }


    public async Task HandleKeyDownAsync(KeyboardEventArgs arg)
    {
        // _logger.Debug("[KeyMapHandler:HandleKeyDownAsync] key down {@KeyboardEventArgs}", arg);

        var keyStrLow = arg.Key.ToLower();

        await BasicFunctions(keyStrLow);
        await MarkupFunctions(keyStrLow,_serviceConfigs.Colors);
 
    }

    private async Task MarkupFunctions(string keyStrLow, ColorModel[] serviceConfigsColors)
    {
        var findKey = serviceConfigsColors.FirstOrDefault(p => p.KeyOnBoard == keyStrLow);
        if(findKey is null)
            return;
    
        await _navigationHandler.HandlerSetLabelIdAsync(findKey.IdLabel);
    }

    private async Task BasicFunctions(string keyStrLow)
    {
        switch (keyStrLow)
        {
            case "d":
            case "arrowleft":
            {
                await _navigationHandler.ButtonGoBackClick();
                break;
            }
            case "f":
            case "arrowright":
            {
                await _navigationHandler.ButtonGoNextClick();
                break;
            }
            case "delete":
            case "x":
            {
                await _navigationHandler.DeleteAnnotation();
                break;
            }
            case "n":
            {
                await _navigationHandler.EventEditAnnot();
                break;
            }
            case "q":
            {
                await _navigationHandler.EnableTypeLabel(TypeLabel.Box);
                break;
            }
            case "w":
            {
                await _navigationHandler.EnableTypeLabel(TypeLabel.Polygon);
                break;
            }
            case "a":
            {
                await _navigationHandler.EnableTypeLabel(TypeLabel.PolyLine);
                break;
            }
            case "s":
            {
                await _navigationHandler.EnableTypeLabel(TypeLabel.Point);
                break;
            }
            case "control":
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
        if (arg.ShiftKey is false)
            await _navigationHandler.HandleImagePanelMouseAsync(arg, DateTime.Now);
    }

    /// <summary>
    ///     Полотно с изображением
    /// </summary>
    /// <param name="arg"></param>
    public async Task HandlerImagesPanelOnmousedownAsync(MouseEventArgs arg)
    {
        if (arg.ShiftKey)
            await _navigationHandler.HandlerImagesPanelOnmousedownAsync(arg,
                DateTime.Now); //кооректируе точку отчета при перемещении изображения

        await _navigationHandler.HandlerSelectPointAsync(arg, DateTime.Now);
    }

    /// <summary>
    ///     Полотно с изображением
    /// </summary>
    /// <param name="arg"></param>
    public async Task HandlerImagesPanelOnmouseupAsync(MouseEventArgs args)
    {
        if (args.Buttons == 1)
            await _navigationHandler.HandlerMovePointAsync(args, DateTime.Now);
    }


    /// <summary>
    ///     Общая панель для отрисовки перемещение мыши
    /// </summary>
    /// <param name="arg"></param>
    public async Task HandlerDrawingPanelOnmousemoveAsync(MouseEventArgs arg)
    {
        if (arg.ShiftKey)
            await _navigationHandler.HandlerDrawingPanelOnmousemoveAsync(arg, DateTime.Now);
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