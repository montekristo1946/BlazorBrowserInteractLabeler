namespace BrowserInteractLabeler.Common.DTO;

public class ExportDTO
{
    public Label[] Labels { get; set; } = [];

    public ImageFrame[] Images { get; set; } = [];

    public Annotation[] Annotations { get; set; } = [];


}