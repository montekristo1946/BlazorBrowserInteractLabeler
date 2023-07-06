using Newtonsoft.Json;
namespace BrowserInteractLabeler.Common.DTO;

public record ImageFrame
{
    public int Id { get; set; }
    [JsonIgnore] public SizeF SizeImage { get; set; } = new() { Height = -1, Width = -1 };
    
    [JsonIgnore] public byte [] Images { get; set; } = Array.Empty<byte>();
    public string NameImages { get; set; } = String.Empty;

    [JsonIgnore] public List<Annotation>? Annotations { get; set; } = new List<Annotation>();

}