using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

namespace BlazorBrowserInteractLabeler.Web.Extensions;

public static class MediatRComponentsBuilder
{

    internal static IHostBuilder UseMediatRComponents(this IHostBuilder builder) => builder
        .ConfigureServices(collection =>
        {
            collection.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(typeof(InitNewAnnotQueries).Assembly));
        });
}
