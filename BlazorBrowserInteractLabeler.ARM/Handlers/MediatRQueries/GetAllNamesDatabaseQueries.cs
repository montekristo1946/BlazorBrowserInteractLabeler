using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class GetAllNamesDatabaseQueries : IRequest<WorksShowerData[]>
{
    public string PathFolderWorkers { get; set; } = string.Empty;
}