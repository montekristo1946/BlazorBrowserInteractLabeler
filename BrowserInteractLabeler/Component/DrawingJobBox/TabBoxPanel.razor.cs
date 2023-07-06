using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Serilog;

namespace BrowserInteractLabeler.Component.DrawingJobBox;

public class LabelsPanelModel : ComponentBase
{
    [Parameter] public Annotation[] AnnotationsOnPanel { get; set; }
    [Parameter] public string HeightMainWin { get; set; } = string.Empty;
    [Parameter] public Label[] LabelAll { get; set; } = Array.Empty<Label>();
    [Parameter] public ColorModel[] ColorAll { get; set; } = Array.Empty<ColorModel>();
    [Parameter] public EventCallback<int> ChoseActiveAnnotationIdAsync { get; set; }
    [Parameter] public EventCallback<int> ChoseActiveLabelIdAsync { get; set; }
    
    internal async Task ButtonClickObjectAsync(int nameIdAnnot)
    {
            await ChoseActiveAnnotationIdAsync.InvokeAsync(nameIdAnnot);
    }
    
    internal async Task ButtonClickLabelAsync(int nameIdAnnot)
    {
        await ChoseActiveLabelIdAsync.InvokeAsync(nameIdAnnot);
    }
    

 
}