using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common;

namespace BlazorBrowserInteractLabeler.Web.Extensions;

public static class ProjectsLocalHandlerBuilder
{
    public static ProjectsLocalHandler Build(IServiceProvider serviceProvider)
    {
        var repository = serviceProvider
            .GetService<IRepository>() ?? throw new ArgumentNullException(nameof(IRepository));

        var markupData = serviceProvider
            .GetService<MarkupData>() ?? throw new ArgumentNullException(nameof(IRepository));
        
        var annotationHandler = serviceProvider.GetService<AnnotationHandler>() ?? throw new ArgumentNullException(nameof(AnnotationHandler));
        
        var handler = new ProjectsLocalHandler(repository,markupData,annotationHandler);

        handler.HandlerChoseActiveDataBaseAsync("/mnt/Disk_D/TMP/17.06.2025/000_2025-06-19_08-11-35-224402.db3").Wait();

        return handler;
    }
}