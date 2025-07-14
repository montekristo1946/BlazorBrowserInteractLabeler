using Newtonsoft.Json;

namespace BrowserInteractLabeler.Common.DTO;

public record PointD
{
    [JsonIgnore]
    public int Id { get; init; }
    public double Y { get; init; } = -1;
    public double X { get; init; } = -1;
    public int PositionInGroup { get; init; } = -1;

    [JsonIgnore]
    public int AnnotationId { get; init; } = -1;
    [JsonIgnore]
    public Annotation Annot { get; init; }



}