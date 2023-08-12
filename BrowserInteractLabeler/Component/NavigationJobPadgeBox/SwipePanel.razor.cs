using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Component;

public class SwipePanelModel:ComponentBase
{
    [Parameter] public EventCallback ButtonGoNextClick { get; set; }
    [Parameter] public EventCallback ButtonGoBackClick { get; set; }

    [Parameter] public string Color { get; set; } = "";

    [Parameter] public string TypeLabel { get; set; } = "";
    
    [Parameter] public string StatePrecess { get; set; } = "";

    [Parameter] public int CurrentIdImg { get; set; } = 0;
    
    [Parameter] public EventCallback<int> ButtonEnterIdActiveIdImages{ get; set; }
    
    private readonly ILogger _logger = Log.ForContext<SwipePanelModel>();

    internal int CurrentIdImgDraw;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            CurrentIdImgDraw = CurrentIdImg;

        //Остановился тут
        // throw new Exception();
    }

    internal async Task EventOninput(ChangeEventArgs arg)
    {
        var resultTryParse = Int32.TryParse((string?)arg.Value, out var indexImg);
        if (!resultTryParse)
            return;
        
        // _logger.Debug($"[EventOninput] {arg.Value} {CurrentIdImgDraw}");
        CurrentIdImgDraw = indexImg;
    }
    
    protected async Task ButtonEnter(KeyboardEventArgs arg)
    {
        if (arg.Key == "Enter")
        {
            // _logger.Debug("[ButtonEnter] {@Value}",IdActiveIdImages);
            await ButtonEnterIdActiveIdImages.InvokeAsync(CurrentIdImgDraw);
            // CurrentIdImgDraw = CurrentIdImg.ToString();
            // StateHasChanged();
        }
    }
    
   
}