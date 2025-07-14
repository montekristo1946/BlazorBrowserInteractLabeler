using System.Globalization;

namespace BlazorBrowserInteractLabeler.Web.Extensions;

public static class SetupCulture
{

    internal static IHostBuilder InitCulture(this IHostBuilder builder)
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", true);
        return builder;
    }
}