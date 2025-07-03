using BrowserInteractLabeler.Common.DTO;

namespace BlazorBrowserInteractLabeler.ARM.ViewData;

public class LabelingPanelDto
{
    
    public int IdAnnotation { get; set; }
    
    public string Color { get; set; }
    
    public string Name { get; set; }
    
    public TypeLabel LabelPattern { get; set; }
    
    public StateAnnot State  { get; set; }
}