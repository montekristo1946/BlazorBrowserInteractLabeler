using System.Text.Json.Serialization;

namespace BrowserInteractLabeler.Common.DTO;

public record Label
{
    [JsonIgnore] public int Id { get; set; }

    public string NameLabel { get; set; } = string.Empty;
}