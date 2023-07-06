using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Pages;

public class ProjectModel:ComponentBase
    
{
    [Inject] internal ProjectsLocalHandler _projectsLocalHandler { get; set; }
   
    // private readonly ILogger _logger = Log.ForContext<MarkerModel>();
    
    
    
    // protected override async Task OnInitializedAsync()
    // {
    //     // _logger.Debug("[ProjectModel:OnInitializedAsync] Load ProjectModel");
    //     //
    //     //
    //     // var pathDb= "/mnt/Disk_D/TMP/20.05.2023/TMP/IMages/14_2023-04-26_13-09-37-635023.db3";
    //     // var resultLoadDatabase =  await Repository.LoadDatabaseAsync(pathDb);
    //     // if (!resultLoadDatabase)
    //     // {
    //     //     _logger.Error("[ProjectModel:OnInitializedAsync] Fail load LoadDatabase");
    //     // }
    //     //
    //     // await _navigationHandler.LoadFirstImg();
    // }
}