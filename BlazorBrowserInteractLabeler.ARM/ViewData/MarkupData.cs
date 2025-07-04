using BlazorBrowserInteractLabeler.ARM.Dto;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;

namespace BlazorBrowserInteractLabeler.ARM.ViewData;

public class MarkupData
{
    private readonly Lock _lock = new Lock();
    private string _imagesUi = string.Empty;
    private SizeT _sizeConvas = new SizeT();
    private double _scaleCurrent  = 1.0F;
    private PointT _offsetDrawImage  = new();
    private SizeWindows _imageMarkerPanelSize = new SizeWindows();
    private CrosshairData _crosshairData = new CrosshairData();
    private int _currentIdImg  = 0;
    private int _currentProgress  = 0;
    private int _allImagesCount = 0;
    private int _labelId = 1;
    private Label[] _labelsName { get; set; } = [];
    private string _nameDb = string.Empty;
    private string _nameImage = string.Empty;
    
    public string NameImage
    {
        get
        {
            lock (_lock)
            {
                return _nameImage;
            }
        }
        set
        {
            lock (_lock)
            {
                _nameImage = value;
            }
        }
    }
    public string NameDb
    {
        get
        {
            lock (_lock)
            {
                return _nameDb;
            }
        }
        set
        {
            lock (_lock)
            {
                _nameDb = value;
            }
        }
    }
    
    public int AllImagesCount
    {
        get
        {
            lock (_lock)
            {
                return _allImagesCount;
            }
        }
        set
        {
            lock (_lock)
            {
                _allImagesCount = value;
            }
        }
    }
    public Label[] LabelsName
    {
        get
        {
            lock (_lock)
            {
                return _labelsName.ToArray();
            }
        }
        set
        {
            lock (_lock)
            {
                _labelsName = value.ToArray();
            }
        }
    }
    public int CurrentLabelId
    {
        get
        {
            lock (_lock)
            {
                return _labelId;
            }
        }
        set
        {
            lock (_lock)
            {
                _labelId = value;
            }
        }
    }
    public int CurrentProgress
    {
        get
        {
            lock (_lock)
            {
                return _currentProgress;
            }
        }
        set
        {
            lock (_lock)
            {
                _currentProgress = value;
            }
        }
    }
    public int CurrentIdImg
    {
        get
        {
            lock (_lock)
            {
                return _currentIdImg;
            }
        }
        set
        {
            lock (_lock)
            {
                _currentIdImg = value;
            }
        } 
    }
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