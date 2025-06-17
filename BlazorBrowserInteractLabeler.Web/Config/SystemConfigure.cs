namespace BlazorBrowserInteractLabeler.Web.Config;


static class SystemConfigure
{
    public static IConfiguration AppSetting { get; }

    static SystemConfigure()
    {
        AppSetting = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Config/SystemConfigure.json")
            .Build();
    }
}
