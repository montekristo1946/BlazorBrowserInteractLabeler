using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Получить все доступные базы с данными для разметки.
/// </summary>
public class GetAllNamesDatabaseHandler: IRequestHandler<GetAllNamesDatabaseQueries, WorksShowerData []>
{
    private readonly ILogger _logger = Log.ForContext<GetAllNamesDatabaseHandler>();
    
    public Task<WorksShowerData[]> Handle(GetAllNamesDatabaseQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return Task.FromResult(Array.Empty<WorksShowerData>());

            var path = request.PathFolderWorkers;
            
            if (!Path.Exists(path))
                return Task.FromResult(Array.Empty<WorksShowerData>());

            var allDbPaths = Directory.GetFiles(path, "*.db3", SearchOption.AllDirectories);
            Array.Sort(allDbPaths);

            var retArr = allDbPaths.Select(p => new WorksShowerData()
            {
                NameDatabes = Path.GetFileName(p),
                PathDatabes = p
            }).ToArray();

            return Task.FromResult(retArr);
        }
        catch (Exception e)
        {
            _logger.Error("[GetAllNamesDatabaseHandler] {@Exception}", e);
        }
        return Task.FromResult(Array.Empty<WorksShowerData>());
    }
}