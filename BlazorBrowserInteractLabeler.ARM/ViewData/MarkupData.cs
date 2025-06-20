using BlazorBrowserInteractLabeler.ARM.Dto;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;

namespace BlazorBrowserInteractLabeler.ARM.ViewData;

public class MarkupData
{
    private readonly Lock _lock = new Lock();

    private string _imagesUi = string.Empty;
    private SizeT _sizeConvas = new SizeT();
    private double _scaleCurrent { get; set; } = 1.0F;
    private PointT _offsetDrawImage { get; set; } = new();

    private SizeWindows _imageMarkerPanelSize = new SizeWindows();
    
    [Obsolete("только для теста сделал")]
    public PointT TestDrawPoint { get; set; }

    private CrosshairData _crosshairData = new CrosshairData();

    public CrosshairData CrosshairData
    {
        get
        {
            lock (_lock)
            {
                return _crosshairData;
            }
        }
        set
        {
            lock (_lock)
            {
                _crosshairData = value;
            }
        } 
    }
    public SizeWindows ImageMarkerPanelSize
    {
        get
        {
            lock (_lock)
            {
                return _imageMarkerPanelSize;
            }
        }
        set
        {
            lock (_lock)
            {
                _imageMarkerPanelSize = value;
            }
        }
    }

    public PointT OffsetDrawImage
    {
        get
        {
            lock (_lock)
            {
                return _offsetDrawImage;
            }
        }
        set
        {
            lock (_lock)
            {
                _offsetDrawImage = value;
            }
        }
    }

    public double ScaleCurrent
    {
        get
        {
            lock (_lock)
            {
                return _scaleCurrent;
            }
        }
        set
        {
            lock (_lock)
            {
                _scaleCurrent = value;
            }
        }
    }

    public SizeT SizeConvas
    {
        get
        {
            lock (_lock)
            {
                return _sizeConvas;
            }
        }
        set
        {
            lock (_lock)
            {
                _sizeConvas = value;
            }
        }
    }

    public string ImagesUI
    {
        get
        {
            lock (_lock)
            {
                return _imagesUi;
            }
        }
        set
        {
            lock (_lock)
            {
                _imagesUi = value;
            }
        }
    }

}