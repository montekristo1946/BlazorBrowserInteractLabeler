using BlazorBrowserInteractLabeler.ARM.Dto;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorBrowserInteractLabeler.ARM.Handlers;

public class MoveImagesHandler
{
    private PointT _firstPosition = new();
    private readonly Lock _lock = new Lock();

    public void HandlerOnmousedown(PointT point)
    {
        lock (_lock)
        {
            _firstPosition = new PointT() { X = point.X, Y = point.Y };
        }
    }


    public (bool res, PointT moveDist) HandlerOnMouseMove(PointT point, double stepCoeff)
    {
        lock (_lock)
        {
            if (_firstPosition.X < 0 || _firstPosition.Y < 0)
            {
                return (false, new PointT());
            }

            var retMove = new PointT()
            {
                X = (point.X - _firstPosition.X) * stepCoeff,
                Y = (point.Y - _firstPosition.Y) * stepCoeff
            };


            if (retMove is { X: 0, Y: 0 })
            {
                return (false, new PointT());
            }

            _firstPosition = new PointT() { X = point.X, Y = point.Y };
            return (true, retMove);
        }
    }
}