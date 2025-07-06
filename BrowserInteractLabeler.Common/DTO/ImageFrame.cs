using Newtonsoft.Json;

namespace BrowserInteractLabeler.Common.DTO;

public record ImageFrame
{
    public int Id { get; init; }
    
    [JsonIgnore] 
    public SizeF SizeImage { get; init; } = new() { Height = -1, Width = -1 };

    [JsonIgnore] 
    public byte[] Images { get; init; } = [];
    public string NameImages { get; init; } = String.Empty;

    [JsonIgnore] public List<Annotation>? Annotations { get; init; } = new();
}