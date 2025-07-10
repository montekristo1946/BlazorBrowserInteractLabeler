using BrowserInteractLabeler.Common.DTO;

namespace BlazorBrowserInteractLabeler.ARM.ViewData;

public class SettingsData
{
    private readonly Lock _lock = new Lock();
    
    private ColorModel[] _colors = [];
    private double _strokeWidth = 2.5;
    private string _pathFolderWorkers = String.Empty;
    private CodeKey [] _codeKeys = [];

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
    
    public CodeKey [] CodeKey
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
                _codeKeys = value ;
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
        CodeKey = InitDefaultCodeKey();
    }

    private  CodeKey[] InitDefaultCodeKey()
    {
        return
        [
            new CodeKey(){CodeFromKeyBoard = "KeyF",KeyOnBoardName = "" , EventCode = EventCode.GoNext},
             new CodeKey(){CodeFromKeyBoard = "ArrowRight",KeyOnBoardName = "" , EventCode = EventCode.GoNext},
             
             new CodeKey(){CodeFromKeyBoard = "KeyD",KeyOnBoardName = "" , EventCode = EventCode.GoBack},
             new CodeKey(){CodeFromKeyBoard = "ArrowLeft",KeyOnBoardName = "" , EventCode = EventCode.GoBack},
             
             new CodeKey(){CodeFromKeyBoard = "Delete",KeyOnBoardName = "" , EventCode = EventCode.DeleteActiveAnnot},
             new CodeKey(){CodeFromKeyBoard = "KeyZ",KeyOnBoardName = "" , EventCode = EventCode.DeleteActiveAnnot},
             
             new CodeKey(){CodeFromKeyBoard = "KeyE",KeyOnBoardName = "" , EventCode = EventCode.SaveAnnotation},
             
             new CodeKey(){CodeFromKeyBoard = "KeyQ",KeyOnBoardName = "" , EventCode = EventCode.InitAnnotationBox},
             new CodeKey(){CodeFromKeyBoard = "KeyW",KeyOnBoardName = "" , EventCode = EventCode.InitAnnotationPolygon},
             new CodeKey(){CodeFromKeyBoard = "KeyA",KeyOnBoardName = "" , EventCode = EventCode.InitAnnotationPolyline},
             new CodeKey(){CodeFromKeyBoard = "KeyS",KeyOnBoardName = "" , EventCode = EventCode.InitAnnotationPoint},
             
             new CodeKey(){CodeFromKeyBoard = "Space",KeyOnBoardName = "" , EventCode = EventCode.MoveDefault},

             new CodeKey(){CodeFromKeyBoard = "Digit1",KeyOnBoardName = "1" , EventCode = EventCode.Label1},
             new CodeKey(){CodeFromKeyBoard = "Digit2",KeyOnBoardName = "2" , EventCode = EventCode.Label2},
             new CodeKey(){CodeFromKeyBoard = "Digit3",KeyOnBoardName = "3" , EventCode = EventCode.Label3},
             new CodeKey(){CodeFromKeyBoard = "Digit4",KeyOnBoardName = "4" , EventCode = EventCode.Label4},
             new CodeKey(){CodeFromKeyBoard = "Digit5",KeyOnBoardName = "5" , EventCode = EventCode.Label5},
             new CodeKey(){CodeFromKeyBoard = "Digit6",KeyOnBoardName = "6" , EventCode = EventCode.Label6},
             new CodeKey(){CodeFromKeyBoard = "Digit7",KeyOnBoardName = "7" , EventCode = EventCode.Label7},
             new CodeKey(){CodeFromKeyBoard = "KeyT",KeyOnBoardName = "t" , EventCode = EventCode.Label8},
             new CodeKey(){CodeFromKeyBoard = "KeyY",KeyOnBoardName = "y" , EventCode = EventCode.Label9},
             new CodeKey(){CodeFromKeyBoard = "KeyU",KeyOnBoardName = "u" , EventCode = EventCode.Label10},
             new CodeKey(){CodeFromKeyBoard = "KeyJ",KeyOnBoardName = "j" , EventCode = EventCode.Label11}
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