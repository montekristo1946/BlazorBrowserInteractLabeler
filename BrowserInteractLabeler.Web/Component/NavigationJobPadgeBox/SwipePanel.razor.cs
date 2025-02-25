using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Web.Component.NavigationJobPadgeBox;

public class SwipePanelModel : ComponentBase
{
    [Parameter] public EventCallback ButtonGoNextClick { get; set; }
    [Parameter] public EventCallback ButtonGoBackClick { get; set; }

    [Parameter] public string Color { get; set; } = "";

    [Parameter] public string TypeLabel { get; set; } = "";

    [Parameter] public string StatePrecess { get; set; } = "";

    [Parameter] public int CurrentIdImg { get; set; } = 0;

    [Parameter] public EventCallback<int> ButtonEnterIdActiveIdImages { get; set; }
    
    [Parameter] public int AllCountImages { get; set; }


    private int _currentIdImgDraw;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            _currentIdImgDraw = CurrentIdImg;
    }

    internal Task EventOninput(ChangeEventArgs arg)
    {
        var resultTryParse = Int32.TryParse((string?)arg.Value, out var indexImg);
        if (!resultTryParse)
            return Task.CompletedTask;
        
        _currentIdImgDraw = indexImg;
        return Task.CompletedTask;
    }

    protected async Task ButtonEnter(KeyboardEventArgs arg)
    {
        if (arg.Key == "Enter")
        {
            await ButtonEnterIdActiveIdImages.InvokeAsync(_currentIdImgDraw);
        }
    }


}