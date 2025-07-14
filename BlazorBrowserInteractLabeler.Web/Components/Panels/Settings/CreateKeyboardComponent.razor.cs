using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Settings;

public partial class CreateKeyboardComponent : ComponentBase
{

    [Inject] private IMediator Mediator { get; set; } = null!;
    [Parameter] public Action? IsNeedUpdateUi { get; set; } = null!;

    private EventCode _eventCode = EventCode.None;
    string _code = String.Empty;
    string _keyOnBoardName = string.Empty;
    string _color = "white";
    private bool _isOpenedPalette = false;

    private EventCode[] GetAllEnums()
    {
        var arrEnum = Enum.GetValues(typeof(EventCode));
        var retValue = new EventCode[arrEnum.Length];
        Array.Copy(arrEnum, retValue, arrEnum.Length);

        return retValue;
    }

    private Task SetCurrentEventCode(ChangeEventArgs changeEventArgs)
    {
        if (changeEventArgs?.Value == null)
            return Task.CompletedTask;

        var textToSave = (string)changeEventArgs.Value! ?? string.Empty;

        var newEnum = Enum.TryParse(textToSave, out EventCode enumValue)
            ? enumValue
            : EventCode.None;

        _eventCode = newEnum;

        return Task.CompletedTask;
    }



    private Task EventOnKeyDown(KeyboardEventArgs arg)
    {
        _keyOnBoardName = arg.Code == "Space" ? arg.Code : arg.Key.ToLower();

        _code = arg.Code;
        return Task.CompletedTask;
    }

    private Task OnInitColorPicker()
    {
        _isOpenedPalette = true;
        return Task.CompletedTask;
    }

    private Task ClosedEventChoiceColor(string color)
    {
        _isOpenedPalette = false;

        if (string.IsNullOrWhiteSpace(color))
            return Task.CompletedTask;

        _color = color;

        return Task.CompletedTask;
    }

    private async Task SaveLabel()
    {
        if (_eventCode == EventCode.None || string.IsNullOrWhiteSpace(_code) || string.IsNullOrWhiteSpace(_keyOnBoardName))
            return;

        var codeKey = new CodeKey()
        {
            CodeFromKeyBoard = _code,
            KeyFromUser = _keyOnBoardName,
            EventCode = _eventCode
        };

        await Mediator.Send(new InitNewCodeKeyQueries() { CodeKey = codeKey, Color = _color });

        _code = string.Empty;
        _keyOnBoardName = string.Empty;
        _color = "white";
        _eventCode = EventCode.None;
        IsNeedUpdateUi?.Invoke();
        StateHasChanged();
    }
}