using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Component;

public class ProgressPanelModel:ComponentBase
{
    [Parameter] public string NameFileEdit { get; set; } = "";
    [Parameter] public int CurrentProgress { get; set; } = 0;
}