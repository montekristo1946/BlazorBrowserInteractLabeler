namespace BrowserInteractLabeler.Common.DTO;

public record InformationDto
{
    public int Id { get; set; } = 0;

    public int CategoryInformation { get; set; } = -1;

    public string Information { get; set; }  = string.Empty;
}