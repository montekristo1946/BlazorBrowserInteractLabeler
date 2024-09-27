using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Pages;

public class ProjectModel : ComponentBase

{
    [Inject] internal ProjectsLocalHandler _projectsLocalHandler { get; set; }

}