using BlazorBrowserInteractLabeler.ARM.Dto;

namespace BlazorBrowserInteractLabeler.ARM.ViewData;

public record CrosshairData
{
    public bool IsShowCrosshair { get; init; } = false;
    
    public PointT PointCursor { get; init; }
    
    public double ScaleCurrent { get; init; }
    
    public string Color { get; init; }

}