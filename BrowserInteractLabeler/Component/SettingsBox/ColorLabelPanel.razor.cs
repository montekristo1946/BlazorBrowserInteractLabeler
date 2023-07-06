using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Component.SettingsBox;

public class ColorLabelPanelModel : ComponentBase
{
    [Parameter] public ColorModel[] ColorModels { get; set; }
    [Parameter] public EventCallback<ColorModel> UpdateColorModel { get; set; }

    internal bool isOpened = false;
    private ColorModel currentColorModel = new ColorModel();


    private readonly ILogger _logger = Log.ForContext<ColorLabelPanelModel>();

    internal Task ChoiceActivePanel(MouseEventArgs mouseEventArgs, ColorModel colorModel)
    {
        currentColorModel = colorModel;
        return Task.CompletedTask;
    }

    internal bool CheckActivePanel(int colorModelIdLabel)
    {
        return colorModelIdLabel == currentColorModel.IdLabel;
    }


    internal Task ClosedEvent(string color)
    {
        if (!color.Any())
            color = "#ffffff";

        isOpened = false;
        currentColorModel.Color = color;


        return Task.CompletedTask;
    }


    internal Task EventOnKeyDown(KeyboardEventArgs arg)
    {
        var keyStrLow = arg.Key.ToLower();
        currentColorModel.KeyOnBoard = keyStrLow;
        return Task.CompletedTask;
    }


    internal async Task ButtonClickSaveLabelInfoAsync(MouseEventArgs arg)
    {
        if (currentColorModel.IdLabel < 0 || currentColorModel.KeyOnBoard == null)
            return;

        currentColorModel.KeyOnBoard = currentColorModel.KeyOnBoard.ToLower();
        await UpdateColorModel.InvokeAsync(currentColorModel);
    }

    internal async Task ClickAddNewPanel(MouseEventArgs arg)
    {
        var colorModelsLast = ColorModels.MaxBy(p => p.IdLabel);

        var colorModel = new ColorModel();
        if (colorModelsLast is null)
            colorModel.IdLabel = 1;

        if (colorModelsLast != null)
            colorModel.IdLabel = colorModelsLast.IdLabel + 1;

        await UpdateColorModel.InvokeAsync(colorModel);
    }
}