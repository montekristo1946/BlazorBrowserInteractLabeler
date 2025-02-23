using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BrowserInteractLabeler.Web.Component.DrawingJobBox;


public partial class  TabBoxPanel : ComponentBase
{
    [Parameter] public Annotation[] AnnotationsOnPanel { get; set; }
    [Parameter] public Label[] LabelAll { get; set; } = Array.Empty<Label>();
    [Parameter] public ColorModel[] ColorAll { get; set; } = Array.Empty<ColorModel>();
    [Parameter] public EventCallback<int> ChoseActiveAnnotationIdAsync { get; set; }
    [Parameter] public EventCallback<int> ChoseActiveLabelIdAsync { get; set; }

    [Parameter] public EventCallback<int> ChoseHiddenLabelIdAsync { get; set; }

    [Parameter] public EventCallback<bool> HiddenAllLabelsAsync { get; set; }

    [Parameter] public SizeF RootWindowsSize { get; set; }

    private bool _isHiddenState = false;


    private string GetHeightPanel()
    {
        const double coef = 0.84d;
        return $"{RootWindowsSize.Height * coef}px";
    }

    private async Task ButtonClickObjectAsync(int nameIdAnnot)
    {
        await ChoseActiveAnnotationIdAsync.InvokeAsync(nameIdAnnot);
    }

    private async Task ButtonClickObjectHiddenAsync(int nameIdAnnot)
    {
        await ChoseHiddenLabelIdAsync.InvokeAsync(nameIdAnnot);
    }

    private async Task ButtonClickObjectHiddenAllAsync(bool isHidden)
    {
        await HiddenAllLabelsAsync.InvokeAsync(isHidden);
    }

    private async Task ButtonClickLabelAsync(int nameIdAnnot)
    {
        await ChoseActiveLabelIdAsync.InvokeAsync(nameIdAnnot);
    }

    private string GetSvgPath(TypeLabel labelPattern)
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

    private bool Switcher()
    {
       
            return _isHiddenState switch
            {
                true => _isHiddenState = false,
                false => _isHiddenState = true};
        
    }
}