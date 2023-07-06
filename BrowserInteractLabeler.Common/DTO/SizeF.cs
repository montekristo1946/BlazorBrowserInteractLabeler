using Newtonsoft.Json;

namespace BrowserInteractLabeler.Common;


public record SizeF
{
    [JsonIgnore]
    public int Id { get; set; }
    public float Height { get; set; }
    public float Width { get; set; }
}