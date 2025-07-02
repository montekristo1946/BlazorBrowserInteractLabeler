using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common;
using MediatR;

namespace BlazorBrowserInteractLabeler.Web.Extensions;

public static class ProjectsLocalHandlerBuilder
{
    public static ProjectsLocalHandler Build(IServiceProvider serviceProvider)
    {
        var repository = serviceProvider.GetService<IRepository>() ??
                         throw new ArgumentNullException(nameof(IRepository));
        var markupData = serviceProvider.GetService<MarkupData>() ??
                         throw new ArgumentNullException(nameof(IRepository));

        var mediator = serviceProvider.GetService<IMediator>() ?? throw new ArgumentNullException(nameof(IMediator));

        var handler = new ProjectsLocalHandler(repository, markupData, mediator);

        var pathDb = "/mnt/Disk_D/TMP/17.06.2025/000_2025-06-19_08-11-35-224402.db3";
        mediator.Send(new ChoseActiveDataBaseQueries() { PathDb = pathDb}).Wait();
        

        return handler;
    }
}