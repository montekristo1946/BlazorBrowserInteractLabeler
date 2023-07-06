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
    private static readonly TimeSpan _minTimeMove = TimeSpan.FromMilliseconds(20);


    public async Task HandlerOnmousedownAsync(MouseEventArgs mouseEventArgs, DateTime timeClick)
    {
        _firstPosition = new PointF() { X = (float)mouseEventArgs.ClientX, Y = (float)mouseEventArgs.ClientY };
        _timeFirstPoint = timeClick;
    }

    public async Task<(bool res, SizeF moveDist)> HandlerOnmouseupAsync(MouseEventArgs mouseEventArgs,
        DateTime timeClick)
    {
        const int step = 50;
        const int minMovePx = 10;
        if (timeClick - _timeFirstPoint < _minTimeMove)
            return (false, new SizeF());

        if (mouseEventArgs.Buttons != 1 || mouseEventArgs.ShiftKey == false)
            return (false, new SizeF());

        var retMove = new SizeF()
        {
            Width = (float)mouseEventArgs.ClientX - _firstPosition.X,
            Height = (float)mouseEventArgs.ClientY - _firstPosition.Y
        };

        if (retMove is { Width: 0, Height: 0 })
            return (false, new SizeF());

        if (Math.Abs(retMove.Width) < minMovePx && Math.Abs(retMove.Height) < 10)
            return (false, new SizeF());

        if (Math.Abs(retMove.Width) > step && retMove.Width > 0)
            retMove.Width = step;

        if (Math.Abs(retMove.Width) > step && retMove.Width < 0)
            retMove.Width = step * (-1);

        if (Math.Abs(retMove.Height) > step && retMove.Height > 0)
            retMove.Height = step;

        if (Math.Abs(retMove.Height) > step && retMove.Height < 0)
            retMove.Height = step * (-1);


        _timeFirstPoint = timeClick;
        _firstPosition = new PointF() { X = (float)mouseEventArgs.ClientX, Y = (float)mouseEventArgs.ClientY };
        // _logger.Debug("[HandlerOnmouseupExternalAsync] {@retMove}", retMove);
        return (true, retMove);
    }
}