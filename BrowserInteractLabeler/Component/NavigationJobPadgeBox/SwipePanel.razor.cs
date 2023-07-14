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
    
    [Parameter]  public string IdActiveIdImages { get; set; } = "";
    
    [Parameter] public EventCallback<string> ButtonEnterIdActiveIdImages{ get; set; }
    
    private readonly ILogger _logger = Log.ForContext<SwipePanelModel>();
    
    internal async Task EventOninput(ChangeEventArgs arg)
    {
        // _logger.Debug("[EventOninput]{@Value}",arg.Value);
        IdActiveIdImages = (string)arg.Value;
    }
    
    protected async Task ButtonEnter(KeyboardEventArgs arg)
    {
        if (arg.Key == "Enter")
        {
            // _logger.Debug("[ButtonEnter] {@Value}",IdActiveIdImages);
            await ButtonEnterIdActiveIdImages.InvokeAsync(IdActiveIdImages);
        }
    }
    
   
}