using BlazorBrowserInteractLabeler.ARM.Dto;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorBrowserInteractLabeler.ARM.Handlers;

public class Helper
{
    internal double CalculationScale(double deltaY, double scaleInput)
    {
        const double maxScale = 5F;
        const double minScale = 0.25F;
        const double stepScale = 0.16f;

        var scale = scaleInput;

        if (deltaY < 0)
            scale *= 1.0f + stepScale;


        if (deltaY > 0)
            scale *= 1.0f - stepScale;


        if (scale < minScale || scale > maxScale) //TODO: проверку offset 
            return scaleInput;

        return scale;
    }


    public PointT GetAbsoluteCoordinate(
        double pageX,
        double pageY,
        SizeWindows panelSize)
    {
        var correctY = pageY - panelSize.Y;
        var correctX = pageX - panelSize.X;

        return new PointT()
        {
            X = correctX, Y = correctY
        };
    }

    public PointT CorrectPoint(PointT point, double scale, PointT offset, SizeT sizeConvas)
    {
        var scaleSizeImg = new SizeT()
        {
            Width = sizeConvas.Width * scale,
            Height = sizeConvas.Height * scale,
        };

        var offsetCorrect = new PointT()
        {
            X = offset.X * scale,
            Y = offset.Y * scale,
        };

        var centerCanvasA = new PointT()
        {
            X = sizeConvas.Width / 2 + offsetCorrect.X,
            Y = sizeConvas.Height / 2 + offsetCorrect.Y,
        };

        var pMin = new PointT()
        {
            X = centerCanvasA.X - scaleSizeImg.Width / 2,
            Y = centerCanvasA.Y - scaleSizeImg.Height / 2,
        };
        

        var pointCorrect = new PointT()
        {
            X = point.X - pMin.X,
            Y = point.Y - pMin.Y,
        };

        var x = pointCorrect.X / scaleSizeImg.Width;
        var y = pointCorrect.Y / scaleSizeImg.Height;

        x = Arround(x);
        y = Arround(y);

        return new PointT()
        {
            X = x,
            Y = y,
        };
    }

    private double Arround(double d)
    {
        return d switch
        {
            < 0 => 0,
            > 1 => 1,
            _ => d
        };
    }

    
}