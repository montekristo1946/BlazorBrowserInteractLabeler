using System.Diagnostics;
using System.Text;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Markup;

public partial class Crosshair : ComponentBase
{
    private CrosshairData? _data = null;

    private string GetTypeLine()
    {
        if (_data == null)
            return  "stroke-dasharray=\"5 4\"";

        var scale = 1/_data.ScaleCurrent;
        var lien = 10*scale;
        var spase = 4*scale;
        return  $"stroke-dasharray=\"{lien} {spase}\"";
    }
    private double GetThicknessLine()
    {
        if (_data == null)
            return 1;
        
        return 0.8 / _data.ScaleCurrent;
    }

    private string GetVerticalLine(CrosshairData? data)
    {
        if (data is null)
            return "";
        
        var x1 = data.PointCursor.X*100;
        var y1 = 0;
        var x2  = data.PointCursor.X*100;
        var y2 = 100;
        var colorModelColor = data.Color;
        var strokeWidth =  GetThicknessLine();
        var typeLine = GetTypeLine();
        var retString =
            $"<line x1=\"{x1}%\" y1=\"{y1}%\" x2=\"{x2}%\"  y2=\"{y2 }%\" stroke=\"{colorModelColor}\" stroke-width=\"{strokeWidth}\" {typeLine}> </line>";

        return retString;
    }
    private string GetHorizontalLine(CrosshairData? data)
    {
        if (data is null)
            return "";

        var x1 = 0;
        var y1 = data.PointCursor.Y*100;
        var x2 = 100;
        var y2 =data.PointCursor.Y*100;
        var colorModelColor = data.Color;
        var strokeWidth =  GetThicknessLine();
        var typeLine = GetTypeLine();
        var retString =
            $"<line x1=\"{x1}%\" y1=\"{y1}%\" x2=\"{x2}%\"  y2=\"{y2 }%\" stroke=\"{colorModelColor}\" stroke-width=\"{strokeWidth}\" {typeLine}> </line>";

        return retString;
    }

    private RenderFragment GetRenderLines() => (builder) =>
    {
        var retStringSvg = new StringBuilder();
        
        retStringSvg.Append(GetVerticalLine(_data));
        retStringSvg.Append(GetHorizontalLine(_data));

        builder.AddMarkupContent(0, retStringSvg.ToString());
    };
    
   
    public void UpdateSvg(CrosshairData data)
    {
        _data = data;
        StateHasChanged();
    }
}