using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components.Web;
using Serilog;

namespace BrowserInteractLabeler.Web.Infrastructure.Handlers;

public class MoveImagesHandler
{
    private readonly Serilog.ILogger _logger = Log.ForContext<NavigationHandler>();

    private PointD _firstPosition = new();
    // private DateTime _timeFirstPoint = DateTime.MinValue;
    // private static readonly TimeSpan _minTimeMove = TimeSpan.FromMilliseconds(1);


    public void HandlerOnmousedown(MouseEventArgs mouseEventArgs, DateTime timeClick)
    {
        _firstPosition = new PointD() { X = (float)mouseEventArgs.ClientX, Y = (float)mouseEventArgs.ClientY };
        // _timeFirstPoint = timeClick;
        // _logger.Debug($"[HandlerOnmousedownAsync] M:{mouseEventArgs.ClientX};{mouseEventArgs.ClientY } ");

    }
    public void HandlerImagesPanelOnmouseUp()
    {
        _firstPosition = new();
    }

    public (bool res, SizeF moveDist) HandlerOnMouseMove(MouseEventArgs mouseEventArgs, double stepCoeff)
    {
        if (_firstPosition.X < 0 || _firstPosition.Y < 0)
            return (false, new SizeF());

        var retMove = new SizeF()
        {
            Width = (float)((mouseEventArgs.ClientX - _firstPosition.X) * stepCoeff),
            Height = (float)((mouseEventArgs.ClientY - _firstPosition.Y) * stepCoeff)
        };

        // _logger.Debug($"[HandlerOnMouseMove] M:{mouseEventArgs.ClientX};{mouseEventArgs.ClientY } F:{_firstPosition.X} {_firstPosition.Y}");


        if (retMove is { Width: 0, Height: 0 })
            return (false, new SizeF());


        _firstPosition = new PointD() { X = (float)mouseEventArgs.ClientX, Y = (float)mouseEventArgs.ClientY };
        return (true, retMove);
    }


}