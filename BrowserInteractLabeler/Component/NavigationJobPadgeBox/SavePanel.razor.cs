using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Component;

public class SavePanelModel : ComponentBase
{
    [Parameter] public EventCallback ButtonSaveClick { get; set; }
    [Parameter] public EventCallback ButtonUndoClick { get; set; }
    [Parameter] public EventCallback ButtonRedoClick { get; set; }
}