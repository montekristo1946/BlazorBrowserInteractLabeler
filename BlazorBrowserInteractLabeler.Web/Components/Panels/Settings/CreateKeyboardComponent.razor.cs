using BlazorBrowserInteractLabeler.ARM.ViewData;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Settings;

public partial class CreateKeyboardComponent : ComponentBase
{
    private EventCode _eventCode = EventCode.None;
    
    private EventCode [] GetAllEnums()
    {
        var arrEnum = Enum.GetValues(typeof(EventCode));
        var retValue= new EventCode[arrEnum.Length];
        Array.Copy(arrEnum, retValue, arrEnum.Length);
        
        return retValue;
    }

    private Task SetCurrentEventCode(ChangeEventArgs changeEventArgs)
    {
        var textToSave = (string)changeEventArgs.Value! ?? string.Empty;

        var newEnum = Enum.TryParse(textToSave, out EventCode enumValue)
            ? enumValue
            : EventCode.None;

        _eventCode = newEnum;
        
        return Task.CompletedTask;
    }

    private Task SetupKey(ChangeEventArgs changeEventArgs)
    {
       
        return Task.CompletedTask;
    }
}