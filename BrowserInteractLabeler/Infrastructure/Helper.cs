using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BrowserInteractLabeler.Infrastructure;

public class Helper
{
    private readonly Serilog.ILogger _logger = Log.ForContext<Helper>();

    public SizeF CalculationOptimalSize(SizeF? imageFrameSize, SizeF? maxSizeDrawWindow)
    {
        if (imageFrameSize is null || imageFrameSize.Height <= 0 || imageFrameSize.Width <= 0 ||
            maxSizeDrawWindow is null || maxSizeDrawWindow.Height <= 0 || maxSizeDrawWindow.Width <= 0)
        {
            _logger.Error("[Helper:CalculationOptimalSize] {ImageFrameSizeImage} {MaxSizeDrawWindow}",
                imageFrameSize, maxSizeDrawWindow);
            return new SizeF() { Height = 100, Width = 100 };
        }

        //сделали изображение подогнаное под одну сторону
        var retWidth = 0F;
        var retHeight = 0F;
        if (imageFrameSize.Width > imageFrameSize.Height)
        {
            retWidth = maxSizeDrawWindow.Width;
            retHeight = maxSizeDrawWindow.Width / imageFrameSize.Width * imageFrameSize.Height;
        }
        else
        {
            retHeight = maxSizeDrawWindow.Height;
            retWidth = maxSizeDrawWindow.Height / imageFrameSize.Height * imageFrameSize.Width;
        }

        //подгоним под реальное окно
        var coef = 1.0F;
        if (retWidth > maxSizeDrawWindow.Width)
        {
            coef = maxSizeDrawWindow.Width / retWidth;
        }

        if (retHeight > maxSizeDrawWindow.Height)
        {
            coef = maxSizeDrawWindow.Height / retHeight;
        }

        retWidth *= coef;
        retHeight *= coef;

        return new SizeF() { Height = retHeight, Width = retWidth };
    }

    public PointF CalculationDefaultOffsetImg(SizeF? imageFrameSize, SizeF? maxSizeDrawWindow)
    {
        if (imageFrameSize is null || imageFrameSize.Height <= 0 || imageFrameSize.Width <= 0 ||
            maxSizeDrawWindow is null || maxSizeDrawWindow.Height <= 0 || maxSizeDrawWindow.Width <= 0)
        {
            _logger.Error("[Helper:CalculationOffsetX] {ImageFrameSizeImage} {MaxSizeDrawWindow}",
                imageFrameSize, maxSizeDrawWindow);
            return new PointF() { Y = 0, X = 0 };
        }

        var offsetX = 0F;
        var offsetY = 0F;
        if (imageFrameSize.Width < maxSizeDrawWindow.Width)
        {
            offsetX = (maxSizeDrawWindow.Width - imageFrameSize.Width) / 2;
        }

        if (imageFrameSize.Height < maxSizeDrawWindow.Height)
        {
            offsetY = (maxSizeDrawWindow.Height - imageFrameSize.Height) / 2;
        }


        return new PointF() { Y = offsetY, X = offsetX };
    }

    internal (float scale, PointF offset) CalculationScale(WheelEventArgs args,
        float? scaleInput,
        SizeF? sizeDrawImage,
        PointF? offsetDrawImage,
        SizeF? imageWindowsSize)
    {
        if (args is null || scaleInput is null || sizeDrawImage is null || offsetDrawImage is null ||
            imageWindowsSize is null)
            return (1, new PointF() { X = 0, Y = 0 });

        const float maxScale = 3F;
        const float minScale = 0.5F;
        const float stepScale = 0.10f;

        var scale = scaleInput;
        var offsetX = offsetDrawImage.X;
        var offsetY = offsetDrawImage.Y;

        if (args.DeltaY < 0)
            scale += stepScale;


        if (args.DeltaY > 0)
            scale -= stepScale;


        if (scale < minScale || scale > maxScale) //TODO: проверку offset 
            return ((float scale, PointF offset))(scaleInput, offsetDrawImage);


        var positionX = offsetDrawImage.X / (1 / scaleInput);
        var offX = positionX * (1 / scale);
        offsetX = (float)offX;

        var positionY = offsetDrawImage.Y / (1 / scaleInput);
        var offY = positionY * (1 / scale);
        offsetY = (float)offY;

        var offset = new PointF() { X = offsetX, Y = offsetY };

        return ((float scale, PointF offset))(scale, offset);
    }

    public PointF CorrectOffset(SizeF? moveDist, PointF? offsetDrawImage)
    {
        if (moveDist is null || offsetDrawImage is null)
        {
            _logger.Error("[Helper:CorrectOffset] Correct Fail: {ImageFrameSizeImage} {MaxSizeDrawWindow}",
                moveDist, offsetDrawImage);
            return new PointF() { Y = 0, X = 0 };
        }

        var offsetX = offsetDrawImage.X + moveDist.Width;
        var offsetY = offsetDrawImage.Y + moveDist.Height;


        return new PointF() { Y = offsetY, X = offsetX };
    }

    public SizeF CalculationRootWindowsSize(SizeF? sizeBrowse)
    {
        const int minHeight = 400;
        const int minWeight = 800;
        const float widthDefault = 0.846f;
        const float heightDefault = 0.90f;

        if (sizeBrowse is null || sizeBrowse.Height < minHeight || sizeBrowse.Width < minWeight)
        {
            _logger.Error("[Helper:CalculationRootWindowsSize]  Fail: {SizeBrowse}}", sizeBrowse);
            return new SizeF() { Width = 1600, Height = 800 };
        }

        var width = sizeBrowse.Width * widthDefault;
        var height = sizeBrowse.Height * heightDefault;

        return new SizeF() { Width = width, Height = height };
    }

    public string CreateTypeTextToPanel(TypeLabel typeLabel)
    {
        return typeLabel switch
        {
            TypeLabel.None => "",
            TypeLabel.Box => "Box",
            TypeLabel.Polygon => "Polygon",
            TypeLabel.PolyLine => "Line",
            TypeLabel.Point => "Point",
            _ => throw new ArgumentOutOfRangeException(nameof(typeLabel), typeLabel, null)
        };
    }

    public string CreateColorTextToPanel(int id, ColorModel[] cacheModelColorAll)
    {
        var colorMap = cacheModelColorAll.FirstOrDefault(p => p.IdLabel == id);
        if (colorMap is null)
            return "";
        return colorMap.Color;
    }

    public int CalculationCurrentProgress(int cacheModelCurrentIdImg, int allIndexLength)
    {
        if (allIndexLength <= 0)
            return 0;
        if (cacheModelCurrentIdImg == allIndexLength)
            return 100;
        var ret = (float)cacheModelCurrentIdImg / (float)allIndexLength * 100F;
        return (int)ret;
    }

    
}