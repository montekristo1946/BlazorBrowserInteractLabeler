using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.LoaderDB;

public partial class WorksShowerComponent : ComponentBase
{
    [Inject] private IMediator Mediator { get; set; } = null!;
    [Inject] private SettingsData SettingsData { get; set; } = null!;
    [Inject] private MarkupData MarkupData { get; set; } = null!;

    private WorksShowerData[] _worksShowerData = [];
    private string _activeDb = string.Empty;
    private bool _enableSpinner = false;
    protected override async Task OnInitializedAsync()
    {
        _worksShowerData = await Mediator.Send(new GetAllNamesDatabaseQueries() { PathFolderWorkers = SettingsData.PathFolderWorkers });
    }
    
    private async Task ClickLoadTask(string path)
    {
        _enableSpinner = true;
        StateHasChanged();
        await Mediator.Send(new ChoseActiveDataBaseQueries() { PathDb = path });
        _enableSpinner = false;
        StateHasChanged();
    }

    private async Task ClickExportTask(string nameDatabes)
    {
        if (_activeDb != nameDatabes)
            return;

        await Mediator.Send(new ExportDatabaseQueries());
    }

    private string GetColorBackground(string nameDatabes)
    {

        return _activeDb == nameDatabes ? "#fdeaee" : string.Empty;
    }

    private string GetBorderStile(string nameDatabes)
    {
        return _activeDb == nameDatabes ? "button-panel" : "settings-panel";
    }

    private bool GetEnableButton(string nameDatabes)
    {

        return _activeDb == nameDatabes ? true : false;
    }

    private void UpdateActiveDb()
    {
        _activeDb = MarkupData.NameDb;

    }

}