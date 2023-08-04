using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Serilog;

namespace BrowserInteractLabeler.Component.DrawingJobBox;

public class LabelsPanelModel : ComponentBase
{
    [Parameter] public Annotation[] AnnotationsOnPanel { get; set; }
    // [Parameter] public string HeightMainWin { get; set; } = string.Empty;
    [Parameter] public Label[] LabelAll { get; set; } = Array.Empty<Label>();
    [Parameter] public ColorModel[] ColorAll { get; set; } = Array.Empty<ColorModel>();
    [Parameter] public EventCallback<int> ChoseActiveAnnotationIdAsync { get; set; }
    [Parameter] public EventCallback<int> ChoseActiveLabelIdAsync { get; set; }
    
    [Parameter] public EventCallback<int> ChoseHiddenLabelIdAsync { get; set; }

    [Parameter] public SizeF RootWindowsSize { get; set; }
    
    
    internal string GetHeightPanel()
    {
        const double coef = 0.84d;
        return $"{RootWindowsSize.Height*coef}px";
    }
    
    internal async Task ButtonClickObjectAsync(int nameIdAnnot)
    {
        await ChoseActiveAnnotationIdAsync.InvokeAsync(nameIdAnnot);
    }
    internal async Task ButtonClickObjectHiddenAsync(int nameIdAnnot)
    {
        await ChoseHiddenLabelIdAsync.InvokeAsync(nameIdAnnot);
    }

    internal async Task ButtonClickLabelAsync(int nameIdAnnot)
    {
        await ChoseActiveLabelIdAsync.InvokeAsync(nameIdAnnot);
    }

    internal string GetSvgPath(TypeLabel labelPattern)
    {
        switch (labelPattern)
        {
            case TypeLabel.None:
                return "icons/014_fail_icon.svg";
            case TypeLabel.Box:
                return "icons/006_rectangle.svg";
            case TypeLabel.Polygon:
                return "icons/007_polygon.svg";
            case TypeLabel.PolyLine:
                return "icons/008_poly_line.svg";
            case TypeLabel.Point:
                return "icons/009_points.svg";
            default:
                throw new ArgumentOutOfRangeException(nameof(labelPattern), labelPattern, null);
        }
        
        
    }
}