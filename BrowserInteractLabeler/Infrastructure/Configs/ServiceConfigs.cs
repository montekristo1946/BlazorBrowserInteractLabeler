using BrowserInteractLabeler.Common.DTO;
using Newtonsoft.Json;
using Serilog;

namespace BrowserInteractLabeler.Infrastructure.Configs;

public class ServiceConfigs
{
    public string PathSqlDb { get; set; } = String.Empty;
    public ColorModel[] Colors { get; set; } = Array.Empty<ColorModel>();
    public string ExportCompletedTasks { get; set; } = String.Empty;
    
    
    
    private static string _pathDirConfigs = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Settings");
    private static string _pathConfig = Path.Join(_pathDirConfigs, $"{nameof(ServiceConfigs)}.json");
    private readonly Serilog.ILogger _logger = Log.ForContext<ServiceConfigs>();

    public static bool SaveInFile(ServiceConfigs conf)
    {
        try
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            var json = JsonConvert.SerializeObject(conf, jsonSerializerSettings);
            File.WriteAllText(_pathConfig, json);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;

    }
    public static ServiceConfigs LoadInFile()
    {
        if (!Directory.Exists(_pathDirConfigs))
            Directory.CreateDirectory(_pathDirConfigs);
        
        if (!File.Exists(_pathConfig))
            SaveInFile(new ServiceConfigs().Init());
        
        
        var config = JsonConvert.DeserializeObject<ServiceConfigs>(File.ReadAllText(_pathConfig));
        if (config is null)
            throw new Exception($"[LoadInFile] fail DeserializeObject {_pathConfig}");

        return config;
    }

    private ServiceConfigs Init()
    {
        PathSqlDb = "/mnt/Disk_D/TMP/29.06.2023/KPTP_cupling/KPTP_DS_001/";
        ExportCompletedTasks = PathSqlDb;
        Colors = InitDefaultColorModel();
        return this;
    }

    private ColorModel[] InitDefaultColorModel()
    {
        return  new[]
        {
            new ColorModel()
            {
                Color = "#F44236",
                IdLabel = 1,
                KeyOnBoardName = "1",
                KeyCode = "Digit1"
                
            },
            new ColorModel()
            {
                Color = "#AA47BC",
                IdLabel = 2,
                KeyOnBoardName = "2",
                KeyCode = "Digit2"
            },
            new ColorModel()
            {
                Color = "#2a1899",
                IdLabel = 3,
                KeyOnBoardName = "3",
                KeyCode = "Digit3"
            },
            new ColorModel()
            {
                Color = "#66BB6A",
                IdLabel = 4,
                KeyOnBoardName = "4",
                KeyCode = "Digit4"
            },
            new ColorModel()
            {
                Color = "#FFEB3C",
                IdLabel = 5,
                KeyOnBoardName = "5",
                KeyCode = "Digit5"
            },
            new ColorModel()
            {
                Color = "#0288D1",
                IdLabel = 6,
                KeyOnBoardName = "6",
                KeyCode = "Digit6"
            },
            new ColorModel()
            {
                Color = "#80CBC4",
                IdLabel = 7,
                KeyOnBoardName = "7",
                KeyCode = "Digit7"
            },
            new ColorModel()
            {
                Color = "#EC407A",
                IdLabel = 8,
                KeyOnBoardName = "8",
                KeyCode = "Digit8"
            },
            new ColorModel()
            {
                Color = "#FF9700",
                IdLabel = 9,
                KeyOnBoardName = "9",
                KeyCode = "Digit9"
            },
            new ColorModel()
            {
                Color = "#D4E056",
                IdLabel = 10,
                KeyOnBoardName = "h",
                KeyCode = "KeyH"
            },
            new ColorModel()
            {
                Color = "#FFCC80",
                IdLabel = 11,
                KeyOnBoardName = "j",
                KeyCode = "KeyJ"
            },
            
            
        };
    }

    public ColorModel GetColor(int annotationLabelId)
    {
        var ret = Colors.FirstOrDefault(p => p.IdLabel == annotationLabelId);
        if (ret is null)
        {
            _logger.Debug("[GetColor] Fail Get color {IdLabel}; ",annotationLabelId );
            return new ColorModel();
        }

        return ret;
    }
}