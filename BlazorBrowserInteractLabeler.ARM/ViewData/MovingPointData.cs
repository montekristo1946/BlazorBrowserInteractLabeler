using BrowserInteractLabeler.Common.DTO;

namespace BlazorBrowserInteractLabeler.ARM.ViewData;

public class MovingPointData
{
    private readonly Lock _lock = new Lock();
    private int _idAnnot = -1;
    public int _positionInGroup = -1;
    private bool _isStartMove = false;

    public bool IsStartMove
    {
        get
        {
            lock (_lock)
            {
                return _isStartMove;
            }
        }
        set
        {
            lock (_lock)
            {
                _isStartMove = value;
            }
        }
    }
    public int PositionInGroup
    {
        get
        {
            lock (_lock)
            {
                return _positionInGroup;
            }
        }
        set
        {
            lock (_lock)
            {
                _positionInGroup = value;
            }
        }
    }

    public int CurrentIdAnnot
    {
        get
        {
            lock (_lock)
            {
                return _idAnnot;
            }
        }
        set
        {
            lock (_lock)
            {
                _idAnnot = value;
            }
        }
    }

    public void HandlerOnmouseDown()
    {

        lock (_lock)
        {
            _isStartMove = true;
        }
    }
    public void HandlerOnmouseUp()
    {

        lock (_lock)
        {
            _isStartMove = false;
        }
    }
}