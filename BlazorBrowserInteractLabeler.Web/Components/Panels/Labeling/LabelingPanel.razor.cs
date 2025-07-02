using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Labeling;

public partial class LabelingPanel : ComponentBase
{
    private bool _isHiddenState = false;
    
    private void ClickHiddenAll(bool isHidden)
    {
        
        
        
    }
    
    private bool Switcher()
    {
       
        return _isHiddenState switch
        {
            true => _isHiddenState = false,
            false => _isHiddenState = true};
        
    }

    private int GetCountAnnots()
    {
        return 999;
    }

    private LabelingPanelDto[] GetAnnots()
    {
        var list = new List<LabelingPanelDto>();
        for (int i = 0; i < 25; i++)
        {
            list.Add( new LabelingPanelDto()
            {
                Color = "red",
                IdAnnotation = i,
                Name = $"{i} heed long name",
                LabelPattern = TypeLabel.Box
            });
        }

        return list.ToArray();
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

    private Task ButtonClickObjectHiddenAsync(int idAnnotation)
    {
        return Task.CompletedTask;
    }

    private Task ButtonClickObjectAsync(int idAnnotation)
    {
        return Task.CompletedTask;
    }
}