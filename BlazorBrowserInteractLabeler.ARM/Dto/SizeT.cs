namespace BlazorBrowserInteractLabeler.ARM.Dto;

public class SizeT
{
    public double Width { get; set; } = 0;
    public double Height { get; set; } = 0;

    public bool IsEmpty()
    {
        return Width <= 0 || Height <= 0;
    }
}