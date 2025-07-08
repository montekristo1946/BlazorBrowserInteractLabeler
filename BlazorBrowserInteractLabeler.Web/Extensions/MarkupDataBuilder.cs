using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.ViewData;

namespace BlazorBrowserInteractLabeler.Web.Extensions;

public static class MarkupDataBuilder
{
    public static MarkupData Build(IServiceProvider serviceProvider)
    {
        // var srcImg = File.ReadAllBytes("/mnt/Disk_D/TMP/17.06.2025/1280x640.jpeg");
        // var srcImg = File.ReadAllBytes("/mnt/Disk_D/TMP/17.06.2025/2100x1436.jpg");
        var srcImg = Properties.Resources.ImagesMoq;
        var base64 = $"data:image/jpg;base64," + Convert.ToBase64String(srcImg);
        return new MarkupData()
        {
            SizeConvas = new SizeT()
            {
                Width = 1800,
                Height = 900,
            },
            CrosshairData = new CrosshairData()
            {
                IsShowCrosshair = true,
                Color = "#ea0d0d"
            },
            ImagesUI = base64

        };
    }
}