using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Web.Component.NavigationJobPadgeBox;

public class ProgressPanelModel : ComponentBase
{
    [Parameter] public string NameFileEdit { get; set; } = "";

    [Parameter] public int CurrentProgress { get; set; } = 0;

    [Parameter] public string CurrentSqlDbName { get; set; } = "";
}