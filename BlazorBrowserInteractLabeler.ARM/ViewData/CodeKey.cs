namespace BlazorBrowserInteractLabeler.ARM.ViewData;

public class CodeKey
{
    public EventCode EventCode { get; set; } = EventCode.None;
    public string KeyFromUser { get; set; } = string.Empty;
    public string CodeFromKeyBoard { get; set; } = string.Empty;
}