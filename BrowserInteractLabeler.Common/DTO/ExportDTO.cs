namespace BrowserInteractLabeler.Common.DTO;

public class ExportDTO
{
    public Label[] Labels { get; set; } = Array.Empty<Label>();

    public ImageFrame [] Images { get; set; } = Array.Empty<ImageFrame>();

    public Annotation[] Annotations { get; set; }= Array.Empty<Annotation>();
    
    
}