using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Восстанавливает позицию изображения.
/// </summary>
public class RestorePositionImageHandler : IRequestHandler<RestorePositionImageQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<RestorePositionImageHandler>();
    private readonly MarkupData _markupData;
    private const double CoefReisizeView = 0.98;
  

    public RestorePositionImageHandler(MarkupData markupData)
    {
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
    }

    public Task<bool> Handle(RestorePositionImageQueries request, CancellationToken cancellationToken)
    {
        try
        {
            var sizeWindows = _markupData.ImageMarkerPanelSize;
            var sizeImg = _markupData.SizeConvas;

            if (sizeWindows.IsEmpty() || sizeImg.IsEmpty())
                return Task.FromResult(false);

            var scaleFull = CalculateScale(sizeWindows, sizeImg);
            var scale = scaleFull * CoefReisizeView;
            _markupData.ScaleCurrent = scale;

            var reversScale = 1 / scale;
            var reverScaleFull = (1 / scaleFull);

            var dWidth = (sizeWindows.Width - (sizeImg.Width * scale)) / 2 * reverScaleFull;
            var dHeight = (sizeWindows.Height - (sizeImg.Height * scale)) / 2 * reverScaleFull;

            var newXOffset = ((sizeImg.Width * scale) - sizeImg.Width) * reversScale / 2 + dWidth;
            var newYOffset = ((sizeImg.Height * scale) - sizeImg.Height) * reversScale / 2 + dHeight;


            _markupData.OffsetDrawImage = new PointT()
            {
                X = newXOffset,
                Y = newYOffset,
            };
            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            _logger.Error("[RestorePositionImageHandler] {@Exception}", e);
        }

        return Task.FromResult(false);
    }

    private double CalculateScale(SizeWindows sizeWindows, SizeT sizeImg)
    {
        var arrScale = new[] { sizeWindows.Width / sizeImg.Width, sizeWindows.Height / sizeImg.Height };
        var minScale = arrScale.Min();
        return minScale;
    }
}