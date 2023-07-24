using BrowserInteractLabeler.Common;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BrowserInteractLabeler.Infrastructure;

public class MoveImagesHandler
{
    private readonly Serilog.ILogger _logger = Log.ForContext<NavigationHandler>();

    private PointF _firstPosition = new();
    private DateTime _timeFirstPoint = DateTime.MinValue;
    private static readonly TimeSpan _minTimeMove = TimeSpan.FromMilliseconds(1);


    public void  HandlerOnmousedownAsync(MouseEventArgs mouseEventArgs, DateTime timeClick)
    {
        _firstPosition = new PointF() { X = (float)mouseEventArgs.ClientX, Y = (float)mouseEventArgs.ClientY };
        _timeFirstPoint = timeClick;
    }

    public (bool res, SizeF moveDist) HandlerOnMouseMove(MouseEventArgs mouseEventArgs, double stepCoeff)
    {
        var retMove = new SizeF()
        {
            Width = (float)((mouseEventArgs.ClientX - _firstPosition.X)*stepCoeff),
            Height = (float)((mouseEventArgs.ClientY - _firstPosition.Y)*stepCoeff)
        };

        if (retMove is { Width: 0, Height: 0 })
            return (false, new SizeF());

       
        _firstPosition = new PointF() { X = (float)mouseEventArgs.ClientX, Y = (float)mouseEventArgs.ClientY };
        return (true, retMove);
    }
}