using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.LoaderDB;

public partial class WorksShowerComponent : ComponentBase
{
    [Inject] private IMediator _mediator { get; set; } = null!;
    [Inject] private SettingsData _settingsData { get; set; } = null!;
    [Inject] private MarkupData _markupData { get; set; } = null!;

    private WorksShowerData[] _worksShowerData = [];
    private string _activeDb = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _worksShowerData =  await _mediator.Send(new GetAllNamesDatabaseQueries() {PathFolderWorkers =_settingsData.PathFolderWorkers});
    }



    private async Task ClickLoadTask(string path)
    {
        await _mediator.Send(new ChoseActiveDataBaseQueries() { PathDb = path});
    }

    private async Task ClickExportTask(string nameDatabes)
    {
        if (_activeDb != nameDatabes)
            return;
        
        await _mediator.Send(new ExportDatabaseQueries() );
    }

    private string GetColorBackground(string nameDatabes)
    {
       
        return _activeDb == nameDatabes ? "#fde5ea" : string.Empty;
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
       _activeDb = _markupData.NameDb;
        
    }

    private string GetShadowButton(string nameDatabes)
    {
        return _activeDb == nameDatabes ? "bottom-shadow" : string.Empty;
        
    }
}