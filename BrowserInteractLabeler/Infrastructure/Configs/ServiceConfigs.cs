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
                Color = "#991897",
                IdLabel = 1,
                KeyOnBoard = "r"
            },
            new ColorModel()
            {
                Color = "#5b1899",
                IdLabel = 2,
                KeyOnBoard = "t"
            },
            new ColorModel()
            {
                Color = "#2a1899",
                IdLabel = 3,
                KeyOnBoard = "y"
            },
            new ColorModel()
            {
                Color = "#186199",
                IdLabel = 4,
                KeyOnBoard = "u"
            },
            new ColorModel()
            {
                Color = "#188899",
                IdLabel = 5,
                KeyOnBoard = "i"
            },
            new ColorModel()
            {
                Color = "#189979",
                IdLabel = 6,
                KeyOnBoard = "o"
            },
            new ColorModel()
            {
                Color = "#18992a",
                IdLabel = 7,
                KeyOnBoard = "p"
            },
            new ColorModel()
            {
                Color = "#6a9918",
                IdLabel = 8,
                KeyOnBoard = "y"
            },
            new ColorModel()
            {
                Color = "#998418",
                IdLabel = 9,
                KeyOnBoard = "g"
            },
            new ColorModel()
            {
                Color = "#994318",
                IdLabel = 10,
                KeyOnBoard = "h"
            },
            new ColorModel()
            {
                Color = "#71731e",
                IdLabel = 11,
                KeyOnBoard = "j"
            },
            new ColorModel()
            {
                Color = "#36298a",
                IdLabel = 12,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#991f18",
                IdLabel = 13,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#f5abe0",
                IdLabel = 14,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#e0abf5",
                IdLabel = 15,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#abb5f5",
                IdLabel = 16,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#abf5f0",
                IdLabel = 17,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#b3f5ab",
                IdLabel = 18,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#f4f5ab",
                IdLabel = 19,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#d42222",
                IdLabel = 20,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#991897",
                IdLabel = 21,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#5b1899",
                IdLabel = 22,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#2a1899",
                IdLabel = 23,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#186199",
                IdLabel = 24,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#188899",
                IdLabel = 25,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#189979",
                IdLabel = 26,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#18992a",
                IdLabel = 27,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#6a9918",
                IdLabel = 28,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#998418",
                IdLabel = 29,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#994318",
                IdLabel = 30,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#71731e",
                IdLabel = 31,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#36298a",
                IdLabel = 32,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#991f18",
                IdLabel = 33,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#f5abe0",
                IdLabel = 34,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#e0abf5",
                IdLabel = 35,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#abb5f5",
                IdLabel = 36,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#abf5f0",
                IdLabel = 37,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#b3f5ab",
                IdLabel = 38,
                KeyOnBoard = ""
            },
            new ColorModel()
            {
                Color = "#f4f5ab",
                IdLabel = 39,
                KeyOnBoard = ""
            },
        };
    }

    public async Task<ColorModel> GetColor(int annotationLabelId)
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