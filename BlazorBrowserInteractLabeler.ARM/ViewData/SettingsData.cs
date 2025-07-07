using BrowserInteractLabeler.Common.DTO;

namespace BlazorBrowserInteractLabeler.ARM.ViewData;

public class SettingsData
{
    private readonly Lock _lock = new Lock();
    private ColorModel[] _colors = [];
    private double _strokeWidth = 2.5;
    private string _pathFolderWorkers = String.Empty;
    private CodeKey [] _codeKeys = [];

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
            new CodeKey(){KeyCode = "KeyF",KeyOnBoardName = "" , EventCode = EventCode.GoNext},
             new CodeKey(){KeyCode = "ArrowRight",KeyOnBoardName = "" , EventCode = EventCode.GoNext},
             
             new CodeKey(){KeyCode = "KeyD",KeyOnBoardName = "" , EventCode = EventCode.GoBack},
             new CodeKey(){KeyCode = "ArrowLeft",KeyOnBoardName = "" , EventCode = EventCode.GoBack},
             
             new CodeKey(){KeyCode = "Delete",KeyOnBoardName = "" , EventCode = EventCode.DeleteActiveAnnot},
             new CodeKey(){KeyCode = "KeyZ",KeyOnBoardName = "" , EventCode = EventCode.DeleteActiveAnnot},
             
             new CodeKey(){KeyCode = "KeyE",KeyOnBoardName = "" , EventCode = EventCode.SaveAnnotation},
             
             new CodeKey(){KeyCode = "KeyQ",KeyOnBoardName = "" , EventCode = EventCode.InitAnnotationBox},
             new CodeKey(){KeyCode = "KeyW",KeyOnBoardName = "" , EventCode = EventCode.InitAnnotationPolygon},
             new CodeKey(){KeyCode = "KeyA",KeyOnBoardName = "" , EventCode = EventCode.InitAnnotationPolyline},
             new CodeKey(){KeyCode = "KeyS",KeyOnBoardName = "" , EventCode = EventCode.InitAnnotationPoint},
             
             new CodeKey(){KeyCode = "Space",KeyOnBoardName = "" , EventCode = EventCode.MoveDefault},

             new CodeKey(){KeyCode = "Digit1",KeyOnBoardName = "1" , EventCode = EventCode.Label1},
             new CodeKey(){KeyCode = "Digit2",KeyOnBoardName = "2" , EventCode = EventCode.Label2},
             new CodeKey(){KeyCode = "Digit3",KeyOnBoardName = "3" , EventCode = EventCode.Label3},
             new CodeKey(){KeyCode = "Digit4",KeyOnBoardName = "4" , EventCode = EventCode.Label4},
             new CodeKey(){KeyCode = "Digit5",KeyOnBoardName = "5" , EventCode = EventCode.Label5},
             new CodeKey(){KeyCode = "Digit6",KeyOnBoardName = "6" , EventCode = EventCode.Label6},
             new CodeKey(){KeyCode = "Digit7",KeyOnBoardName = "7" , EventCode = EventCode.Label7},
             new CodeKey(){KeyCode = "KeyT",KeyOnBoardName = "t" , EventCode = EventCode.Label8},
             new CodeKey(){KeyCode = "KeyY",KeyOnBoardName = "y" , EventCode = EventCode.Label9},
             new CodeKey(){KeyCode = "KeyU",KeyOnBoardName = "u" , EventCode = EventCode.Label10},
             new CodeKey(){KeyCode = "KeyJ",KeyOnBoardName = "j" , EventCode = EventCode.Label11}
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