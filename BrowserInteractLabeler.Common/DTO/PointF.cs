using Newtonsoft.Json;

namespace BrowserInteractLabeler.Common.DTO;

public record PointF
{
    [JsonIgnore]
    public int Id { get; init; }
    public float Y { get; init; } = -1;
    public float X { get; init; }= -1;
    public int PositionInGroup { get; init; } = -1;
    
    [JsonIgnore]
    public int AnnotationId { get; init; } = -1;
    [JsonIgnore]
    public Annotation Annot { get; init; }
    
    
    
}