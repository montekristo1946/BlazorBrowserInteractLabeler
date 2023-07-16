using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.AspNetCore.Components.Web;

namespace BrowserInteractLabeler.Infrastructure;

public class CursorHandler
{
    private DateTime _lastMove = DateTime.MinValue;
    private static readonly TimeSpan _minTimeMove = TimeSpan.FromMilliseconds(100);

    public (bool checkres, PointF drawPoint) GetPointDraw(MouseEventArgs mouseEventArgs, DateTime dateTime,
        SizeF sizeImg)
    {
        if (dateTime-_lastMove < _minTimeMove)
            return (false, new PointF());

        var pointClick = new PointF()
        {
            X = (float)mouseEventArgs.OffsetX / sizeImg.Width,
            Y = (float)mouseEventArgs.OffsetY / sizeImg.Height,
            Annot = new Annotation()
        };
        _lastMove = dateTime;
        return (true, pointClick);
    }
}