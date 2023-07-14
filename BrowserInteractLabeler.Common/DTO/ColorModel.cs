namespace BrowserInteractLabeler.Common.DTO;

public record ColorModel
{
    public int IdLabel { get; set; } = -1;
    public string Color { get; set; } = "#ffffff";

    public string? KeyOnBoardName { get; set; } = "";
    
    public string? KeyCode{ get; set; } = "";
    
}