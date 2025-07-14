using Newtonsoft.Json;

namespace BrowserInteractLabeler.Common.DTO;


public record Annotation
{
    public int Id { get; set; }

    public int LabelId { get; set; }

    public List<PointD>? Points { get; set; } = [];

    public TypeLabel LabelPattern { get; set; } = TypeLabel.None;

    [JsonIgnore]
    public StateAnnot State { get; set; } = StateAnnot.Finalized;

    public int ImageFrameId { get; set; } = -1;

}