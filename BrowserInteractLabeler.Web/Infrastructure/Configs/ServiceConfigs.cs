using BrowserInteractLabeler.Common.DTO;
using Newtonsoft.Json;
using Serilog;

namespace BrowserInteractLabeler.Web.Infrastructure.Configs;

public class ServiceConfigs
{
    public string PathSqlDb { get; set; } = String.Empty;
    public ColorModel[] Colors { get; set; } = Array.Empty<ColorModel>();
    public string ExportCompletedTasks { get; set; } = String.Empty;

    private double _strokeWidth = 2.5;
    public double StrokeWidth
    {
        get => _strokeWidth;
        set => _strokeWidth = value is >= 0.5 and <= 10 ? value : 2.5;

    }


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
        PathSqlDb = "./Example";
        ExportCompletedTasks = PathSqlDb;
        Colors = InitDefaultColorModel();
        return this;
    }

    private ColorModel[] InitDefaultColorModel()
    {
        return new[]
        {
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
            },


        };
    }

    public ColorModel GetColor(int annotationLabelId)
    {
        var ret = Colors.FirstOrDefault(p => p.IdLabel == annotationLabelId);
        if (ret is null)
        {
            // _logger.Debug("[GetColor] Fail Get color {IdLabel}; ",annotationLabelId );
            return new ColorModel();
        }

        return ret;
    }
}