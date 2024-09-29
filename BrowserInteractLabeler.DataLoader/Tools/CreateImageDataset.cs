using System.Collections.Concurrent;
using System.Net.Mime;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Repository;
using ImageMagick;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace BrowserInteractLabeler.DataLoader.Tools;

public class CreateImageDataset : IHostedService
{
    private readonly string _pathImg;
    private readonly string _typeWork;
    private static readonly int _parallelism = Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount * 0.75));
    private readonly ILogger _logger = Log.ForContext<CreateImageDataset>();


    public CreateImageDataset(string pathImg, string typeWork)
    {
        _pathImg = pathImg ?? throw new ArgumentNullException(nameof(pathImg));
        _typeWork = typeWork ?? throw new ArgumentNullException(nameof(typeWork));
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        switch (_typeWork)
        {
            case "loadImg":
            {
                await LoadDataset(_pathImg);
                break;
            }
            default:
            {
                Helper.HelperPrint();
                throw new NotImplementedException($"Not Found {_typeWork}");
            }
        }

        Environment.Exit(0); //костыляка из зависания сериалога
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }

    private async Task LoadDataset(string pathTask)
    {
        var directories = Directory.GetDirectories(pathTask);
        if (!directories.Any())
        {
            _logger.Error("[LoadDataset] fail GetDirectories");
            return;
        }

        foreach (var taskFolder in directories)
        {
            _logger.Debug("[LoadDataset] Load Folder:{ImageName}", taskFolder);

            var nameDb = Path.GetFileName(taskFolder);
            var pathSaveDB = Path.GetDirectoryName(taskFolder);
            if (pathSaveDB is null)
                throw new Exception($"[LoadDataset] Fail GetDirectoryName {taskFolder}");

            var fullPathDb = Path.Combine(pathSaveDB, $"{nameDb}.db3");

            // using var sqlContext = new ApplicationDbContext(fullPathDb);
            using IRepository operativeFramesStorage = new SqlRepository();
            operativeFramesStorage.LoadDatabase(fullPathDb);
            await LoadImages(taskFolder, operativeFramesStorage).ConfigureAwait(false);
            await LoadLabels(taskFolder, operativeFramesStorage).ConfigureAwait(false);
            await LoadAnnotations(taskFolder, operativeFramesStorage).ConfigureAwait(false);
        }
    }

    private async Task<ExportDTO> GetDtoToJson(string taskFolder)
    {
        var allLabelsPath = Directory.GetFiles(taskFolder, "*.json", SearchOption.AllDirectories);
        var firstLabel = allLabelsPath.FirstOrDefault();
        if (firstLabel is null)
            throw new Exception($"[GetDtoToJson] Fail load *.json Labels, folder: {taskFolder}");

        var exportDto = JsonConvert.DeserializeObject<ExportDTO>(await File.ReadAllTextAsync(firstLabel));
        if (exportDto is null)
            throw new Exception($"[GetDtoToJson] Fail DeserializeObject, folder: {taskFolder}");
        return exportDto;
    }

    private async Task LoadAnnotations(string taskFolder, IRepository repository)
    {
        var exportDto = await GetDtoToJson(taskFolder);
        if (!exportDto.Annotations.Any() || !exportDto.Images.Any())
            return;

        var annots = exportDto.Annotations;
        var frames = repository.GetAllImages();
        var annotGroupByImgImport = annots.GroupBy(p => p.ImageFrameId)
            .Select(groupAnnot =>
            {
                var importImg = exportDto.Images.FirstOrDefault(p => p.Id == groupAnnot.Key);
                if (importImg is null)
                    throw new ArgumentException($"fail {groupAnnot.Key}");
                var nameImages = Path.GetFileNameWithoutExtension(importImg.NameImages);
                return (nameImages, groupAnnot);
            }).ToArray();

        foreach (var frameInBase in frames)
        {
            var nameInBase = Path.GetFileNameWithoutExtension(frameInBase.NameImages);
            var indexInBase = frameInBase.Id;
            var annotsImport = annotGroupByImgImport
                .Where(p => p.nameImages == nameInBase)
                .SelectMany(p => p.groupAnnot)
                .Select(annot =>
                {
                    return annot with
                    {
                        Id = 0, ImageFrameId = indexInBase,
                        Points = annot.Points?.Select(p => p with { Id = 0 }).ToList()
                    };
                })
                .ToArray();
            if (!annotsImport.Any())
                continue;

            var res = repository.SaveAnnotations(annotsImport);
            if (!res)
                throw new Exception($"[GetDtoToJson] Fail SaveAnnotationsAsync, folder: {taskFolder}");
        }
    }

    private async Task LoadLabels(string taskFolder, IRepository operativeFramesStorage)
    {
        var exportDto = await GetDtoToJson(taskFolder);
        if (!exportDto.Labels.Any())
            throw new Exception($"[LoadLabels] Fail Labels, folder: {taskFolder}");

        var res = operativeFramesStorage.InsertLabels(exportDto.Labels);
        if (!res)
            throw new Exception($"[LoadDataset] Fail InsertLabel, name {taskFolder}");
    }

    private (int width, int height, byte[] img) ResizeImg(byte[] img, string pathImg)
    {
        const int optimalHeightOnInterface = 640;
        const int optimalWidthOnInterface = 1280;
        try
        {
            using var image = new MagickImage(img);
            image.Resize(optimalWidthOnInterface, optimalHeightOnInterface);
            image.Quality = 95;
            image.Format = MagickFormat.Jpg;
            var data = image.ToByteArray();
            var width = image.Width;
            var height = image.Height;
            return (width, height, data);
        }
        catch (Exception e)
        {
            _logger.Error("Error: File is corrupted: {PathImg} ", pathImg);
            throw;
        }
    }

    private async Task LoadImages(string taskFolder, IRepository operativeFramesStorage)
    {
        var allImagesPath = Directory.GetFiles(taskFolder, "*.jp*g", SearchOption.AllDirectories);

        var arrFrames = GetPrimeNumbersParallel(allImagesPath);
        arrFrames = arrFrames.OrderBy(p => p.Key).ToArray();

        var onlyFrame = arrFrames.Select(p => p.Value).ToArray();

        var res = operativeFramesStorage.InsertImageFrames(onlyFrame);
        if (!res)
            throw new Exception($"[LoadDataset] Fail InsertImageFrame, taskFolder: {taskFolder}");
    }

    private KeyValuePair<string, ImageFrame>[] GetPrimeNumbersParallel(string[] pathImages)
    {
        var retDict = new ConcurrentDictionary<string, ImageFrame>();
        var options = new ParallelOptions() { MaxDegreeOfParallelism = _parallelism };

        Parallel.ForEach(pathImages, options, pathImg =>
        {
            var img = File.ReadAllBytes(pathImg);
            var resizeImg = ResizeImg(img, pathImg);
            var imageFrame = new ImageFrame()
            {
                Images = resizeImg.img,
                NameImages = Path.GetFileName(pathImg),
                SizeImage = new SizeF() { Width = resizeImg.width, Height = resizeImg.height },
                Annotations = new List<Annotation>()
            };

            retDict.TryAdd(pathImg, imageFrame);
        });
        return retDict.ToArray();
    }
}