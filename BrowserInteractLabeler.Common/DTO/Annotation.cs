using Newtonsoft.Json;

namespace BrowserInteractLabeler.Common.DTO;


public record Annotation
{
    public int Id { get; set; } 

    public int LabelId { get; set; } 

    public List<PointF>? Points { get; set; } = new();

    public TypeLabel LabelPattern { get; set; } = TypeLabel.None;

    [JsonIgnore]
    public StateAnnot State { get; set; } = StateAnnot.Finalized;
    
    public int ImageFrameId { get; set; } = -1;
    
    [JsonIgnore]
    public ImageFrame Images { get; set; } 
    
}