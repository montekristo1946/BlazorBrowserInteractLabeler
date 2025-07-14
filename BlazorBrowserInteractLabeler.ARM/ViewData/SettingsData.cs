using BrowserInteractLabeler.Common.DTO;

namespace BlazorBrowserInteractLabeler.ARM.ViewData;

public class SettingsData
{
    private readonly Lock _lock = new Lock();

    private ColorModel[] _colors = [];
    private double _strokeWidth = 2.5;
    private string _pathFolderWorkers = String.Empty;
    private CodeKey[] _codeKeys = [];

    private static string _pathDirConfigs = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Config");
    private static string _pathConfig = Path.Join(_pathDirConfigs, $"{nameof(SettingsData)}.json");


    public string PathDirConfigs
    {
        get
        {
            lock (_lock)
            {
                return _pathConfig;
            }
        }
    }

    public CodeKey[] CodeKey
    {
        get
        {
            lock (_lock)
            {
                return _codeKeys;
            }
        }
        set
        {
            lock (_lock)
            {
                _codeKeys = value;
            }
        }
    }

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
                _pathFolderWorkers = value;
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
        CodeKey = InitDefaultCodeKey();
    }

    private CodeKey[] InitDefaultCodeKey()
    {
        return
        [
            new CodeKey(){CodeFromKeyBoard = "KeyF",KeyFromUser = "f" , EventCode = EventCode.GoNext},
             new CodeKey(){CodeFromKeyBoard = "ArrowRight",KeyFromUser = "ArrowRight" , EventCode = EventCode.GoNext},

             new CodeKey(){CodeFromKeyBoard = "KeyD",KeyFromUser = "d" , EventCode = EventCode.GoBack},
             new CodeKey(){CodeFromKeyBoard = "ArrowLeft",KeyFromUser = "ArrowLeft" , EventCode = EventCode.GoBack},

             new CodeKey(){CodeFromKeyBoard = "Delete",KeyFromUser = "del" , EventCode = EventCode.DeleteActiveAnnot},
             new CodeKey(){CodeFromKeyBoard = "KeyZ",KeyFromUser = "z" , EventCode = EventCode.DeleteActiveAnnot},

             new CodeKey(){CodeFromKeyBoard = "KeyE",KeyFromUser = "e" , EventCode = EventCode.SaveAnnotation},

             new CodeKey(){CodeFromKeyBoard = "KeyQ",KeyFromUser = "q" , EventCode = EventCode.InitAnnotationBox},
             new CodeKey(){CodeFromKeyBoard = "KeyW",KeyFromUser = "w" , EventCode = EventCode.InitAnnotationPolygon},
             new CodeKey(){CodeFromKeyBoard = "KeyA",KeyFromUser = "a" , EventCode = EventCode.InitAnnotationPolyline},
             new CodeKey(){CodeFromKeyBoard = "KeyS",KeyFromUser = "s" , EventCode = EventCode.InitAnnotationPoint},

             new CodeKey(){CodeFromKeyBoard = "Space",KeyFromUser = "Space" , EventCode = EventCode.MoveDefault},

             new CodeKey(){CodeFromKeyBoard = "Digit1",KeyFromUser = "1" , EventCode = EventCode.Label1},
             new CodeKey(){CodeFromKeyBoard = "Digit2",KeyFromUser = "2" , EventCode = EventCode.Label2},
             new CodeKey(){CodeFromKeyBoard = "Digit3",KeyFromUser = "3" , EventCode = EventCode.Label3},
             new CodeKey(){CodeFromKeyBoard = "Digit4",KeyFromUser = "4" , EventCode = EventCode.Label4},
             new CodeKey(){CodeFromKeyBoard = "Digit5",KeyFromUser = "5" , EventCode = EventCode.Label5},
             new CodeKey(){CodeFromKeyBoard = "Digit6",KeyFromUser = "6" , EventCode = EventCode.Label6},
             new CodeKey(){CodeFromKeyBoard = "Digit7",KeyFromUser = "7" , EventCode = EventCode.Label7},
             new CodeKey(){CodeFromKeyBoard = "KeyT",KeyFromUser = "t" , EventCode = EventCode.Label8},
             new CodeKey(){CodeFromKeyBoard = "KeyY",KeyFromUser = "y" , EventCode = EventCode.Label9},
             new CodeKey(){CodeFromKeyBoard = "KeyU",KeyFromUser = "u" , EventCode = EventCode.Label10},
             new CodeKey(){CodeFromKeyBoard = "KeyJ",KeyFromUser = "j" , EventCode = EventCode.Label11}
        ];
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
            },
            new ColorModel()
            {
                Color = "#8BC24A",
                IdLabel = 2,
            },
            new ColorModel()
            {
                Color = "#B61C1C",
                IdLabel = 3,
            },
            new ColorModel()
            {
                Color = "#F9BBD0",
                IdLabel = 4,

            },
            new ColorModel()
            {
                Color = "#FF6F00",
                IdLabel = 5,

            },
            new ColorModel()
            {
                Color = "#FFEB3C",
                IdLabel = 6,

            },
            new ColorModel()
            {
                Color = "#1C5E20",
                IdLabel = 7,

            },
            new ColorModel()
            {
                Color = "#6A1B9A",
                IdLabel = 8,

            },
            new ColorModel()
            {
                Color = "#303E9F",
                IdLabel = 9,

            },
            new ColorModel()
            {
                Color = "#EC407A",
                IdLabel = 10,

            },
            new ColorModel()
            {
                Color = "#FFCC80",
                IdLabel = 11,
            }
        ];
    }


}