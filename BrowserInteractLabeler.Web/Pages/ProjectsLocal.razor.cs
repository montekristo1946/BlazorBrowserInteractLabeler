using BrowserInteractLabeler.Web.Infrastructure.Handlers;
using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Web.Pages;

public class ProjectModel : ComponentBase

{
    [Inject] internal ProjectsLocalHandler _projectsLocalHandler { get; set; }

}