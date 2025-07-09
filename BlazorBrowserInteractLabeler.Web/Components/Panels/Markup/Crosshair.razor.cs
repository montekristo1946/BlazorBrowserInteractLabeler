using System.Diagnostics;
using System.Text;
using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Markup;

public partial class Crosshair : ComponentBase
{
    private CrosshairData? _data = null;
    private const string BackgroindColor = "white";

    private string GetTypeLine(double scale)
    {
        if (scale == 0)
            return "stroke-dasharray=\"5 4\"";

        var scaleNew = 1 / scale;
        var lien = 10 * scaleNew;
        var spase = 4 * scaleNew;
        return $"stroke-dasharray=\"{lien} {spase}\"";
    }

    private double GetThicknessLine(double scale)
    {
        if (scale == 0)
            return 1;

        return 0.8 / scale;
    }

    private string GetHorizontalLine(string color, PointT cursorPoint, double scale)
    {
        var x1 = 0;
        var y1 = cursorPoint.Y * 100;
        var x2 = 100;
        var y2 = cursorPoint.Y * 100;
        var strokeWidth = GetThicknessLine(scale);
        var typeLine = GetTypeLine(scale);
        var retString =
            $"<line x1=\"{x1}%\" y1=\"{y1}%\" x2=\"{x2}%\"  y2=\"{y2}%\" stroke=\"{color}\" stroke-width=\"{strokeWidth}\" {typeLine}> </line>";

        return retString;
    }

    private RenderFragment GetRenderLines() => (builder) =>
    {
        var retStringSvg = new StringBuilder();
        if (_data is not null)
        {
            retStringSvg.Append(GetVerticalLine(BackgroindColor, _data.PointCursor, _data.ScaleCurrent));
            retStringSvg.Append(GetVerticalLine(_data.Color, _data.PointCursor, _data.ScaleCurrent));

            retStringSvg.Append(GetHorizontalLine(BackgroindColor, _data.PointCursor, _data.ScaleCurrent));
            retStringSvg.Append(GetHorizontalLine(_data.Color, _data.PointCursor, _data.ScaleCurrent));
        }

        builder.AddMarkupContent(0, retStringSvg.ToString());
    };

    private string GetVerticalLine(string color, PointT cursorPoint, double scale)
    {
        var x1 = cursorPoint.X * 100;
        var y1 = 0;
        var x2 = cursorPoint.X * 100;
        var y2 = 100;
        var strokeWidth = GetThicknessLine(scale);
        var typeLine = GetTypeLine(scale);
        var retString =
            $"<line x1=\"{x1}%\" y1=\"{y1}%\" x2=\"{x2}%\"  y2=\"{y2}%\" stroke=\"{color}\" stroke-width=\"{strokeWidth}\" {typeLine}> </line>";

        return retString;
    }
    public void UpdateSvg(CrosshairData data)
    {
        _data = data;
        StateHasChanged();
    }
}