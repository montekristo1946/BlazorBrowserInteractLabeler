using Newtonsoft.Json;

namespace BrowserInteractLabeler.Common.DTO;


public record SizeF
{
    [JsonIgnore]
    public int Id { get; set; }
    public float Height { get; set; }
    public float Width { get; set; }
}