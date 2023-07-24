using BrowserInteractLabeler.Common.DTO;
using Newtonsoft.Json;

namespace BrowserInteractLabeler.Common;

public record PointF
{
    [JsonIgnore]
    public int Id { get; set; }
    public float Y { get; set; } = -1;
    public float X { get; set; }= -1;

    public int PositionInGroup { get; set; } = -1;
    
    [JsonIgnore]
    public int AnnotationId { get; set; } = -1;
    [JsonIgnore]
    public Annotation Annot { get; set; }
    
    
    
}