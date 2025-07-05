using BrowserInteractLabeler.Common.DTO;

namespace BlazorBrowserInteractLabeler.ARM.ViewData;

public class MovingPointData
{
    private readonly Lock _lock = new Lock();
    private int _idAnnot = -1;
    public int _positionInGroup = -1;
    
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


}