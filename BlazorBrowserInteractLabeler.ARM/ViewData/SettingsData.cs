using BrowserInteractLabeler.Common.DTO;

namespace BlazorBrowserInteractLabeler.ARM.ViewData;

public class SettingsData
{
    private readonly Lock _lock = new Lock();
    private ColorModel[] _colors = [];
    private double _strokeWidth = 2.5;
    private string _pathFolderWorkers = String.Empty;
    
    public string PathFolderWorkers
    {
        get
        {
            lock (_lock)
            {
                return _pathFolderWorkers;
            }
        }
        set
        {
            lock (_lock)
            {
                _pathFolderWorkers = value ;
            }
        }
    }
    public double StrokeWidth
    {
        get
        {
            lock (_lock)
            {
                return _strokeWidth;
            }
        }
        set
        {
            lock (_lock)
            {
                _strokeWidth = value is >= 0.5 and <= 10 ? value : 2.5;
            }
        }
    }

    public ColorModel[] ColorModel
    {
        get
        {
            lock (_lock)
            {
                return _colors.ToArray();
            }
        }
        set
        {
            lock (_lock)
            {
                _colors = value.ToArray();
            }
        }
    }

    public void Init()
    {
        ColorModel = InitDefaultColorModel();
    }

    public ColorModel GetColor(int annotationLabelId)
    {
        var ret = ColorModel.FirstOrDefault(p => p.IdLabel == annotationLabelId);
        return ret ?? new ColorModel();
    }
    
    private ColorModel[] InitDefaultColorModel()
    {
        return
        [
            new ColorModel()
            {
                Color = "#2196F3",
                IdLabel = 1,
                KeyOnBoardName = "1",
                KeyCode = "Digit1"
            },
            new ColorModel()
            {
                Color = "#8BC24A",
                IdLabel = 2,
                KeyOnBoardName = "2",
                KeyCode = "Digit2"
            },
            new ColorModel()
            {
                Color = "#B61C1C",
                IdLabel = 3,
                KeyOnBoardName = "3",
                KeyCode = "Digit3"
            },
            new ColorModel()
            {
                Color = "#F9BBD0",
                IdLabel = 4,
                KeyOnBoardName = "4",
                KeyCode = "Digit4"
            },
            new ColorModel()
            {
                Color = "#FF6F00",
                IdLabel = 5,
                KeyOnBoardName = "5",
                KeyCode = "Digit5"
            },
            new ColorModel()
            {
                Color = "#FFEB3C",
                IdLabel = 6,
                KeyOnBoardName = "6",
                KeyCode = "Digit6"
            },
            new ColorModel()
            {
                Color = "#1C5E20",
                IdLabel = 7,
                KeyOnBoardName = "7",
                KeyCode = "Digit7"
            },
            new ColorModel()
            {
                Color = "#6A1B9A",
                IdLabel = 8,
                KeyOnBoardName = "t",
                KeyCode = "KeyT"
            },
            new ColorModel()
            {
                Color = "#303E9F",
                IdLabel = 9,
                KeyOnBoardName = "y",
                KeyCode = "KeyY"
            },
            new ColorModel()
            {
                Color = "#EC407A",
                IdLabel = 10,
                KeyOnBoardName = "u",
                KeyCode = "KeyU"
            },
            new ColorModel()
            {
                Color = "#FFCC80",
                IdLabel = 11,
                KeyOnBoardName = "j",
                KeyCode = "KeyJ"
            }
        ];
    }


}