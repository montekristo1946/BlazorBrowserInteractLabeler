using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using ImageMagick;
using Serilog;
using PointF = BrowserInteractLabeler.Common.PointF;
using SizeF = BrowserInteractLabeler.Common.SizeF;

namespace BrowserInteractLabeler.Repository;

public class MockupRepository : IRepository
{
    private readonly ILogger _logger = Log.ForContext<MockupRepository>();
    private string[] _allImagesPath = Array.Empty<string>();
    private readonly Label[] _labels;
    private List<Annotation> _annotations = new List<Annotation>();

    public MockupRepository()
    {
        _labels = CreateDefaultLabels();
        _annotations = CreateDefaultAnnotation();
    }

    private List<Annotation> CreateDefaultAnnotation()
    {
        return new List<Annotation>()
        {
            new Annotation()
            {
                Id = 0, ImageFrameId = 0,
                Points = new List<PointF>()
                    { new PointF() { X = 0.5f, Y = 0.5f }, new PointF() { X = 0.6f, Y = 0.56f } },
                LabelId = 2, LabelPattern = TypeLabel.Box, State =  StateAnnot.Finalized
                
            },
            new Annotation()
            {
                Id = 1, ImageFrameId = 0,
                Points = new List<PointF>()
                    { new PointF() { X = 0.1f, Y = 0.1f }, new PointF() { X = 0.2f, Y = 0.2f } },
                LabelId = 7, LabelPattern = TypeLabel.Box, State =  StateAnnot.Finalized
                
            },
            new Annotation()
            {
                Id = 2, ImageFrameId = 7,
                Points = new List<PointF>()
                    { new PointF() { X = 0.1f, Y = 0.1f }, new PointF() { X = 0.6f, Y = 0.56f } },
                LabelId = 3, LabelPattern = TypeLabel.Box, State =  StateAnnot.Finalized
            }
        };
    }

    private Label[] CreateDefaultLabels()
    {
        return new[]
        {
            new Label() { Id = 0, NameLabel = "йод" },
            new Label() { Id = 1, NameLabel = "Dog" },
            new Label() { Id = 2, NameLabel = "русский_длиниый_текст_и_еще_паруСлов" },
            new Label() { Id = 3, NameLabel = "русский с разделитями" },
            new Label() { Id = 4, NameLabel = "Тест_4" },
            new Label() { Id = 5, NameLabel = "Тест_5" },
            new Label() { Id = 6, NameLabel = "Тест_6" },
            new Label() { Id = 7, NameLabel = "Тест_7" },
            new Label() { Id = 8, NameLabel = "Тест_8" },
            new Label() { Id = 9, NameLabel = "Тест_9" },
            new Label() { Id = 10, NameLabel = "Тест_10" },
            new Label() { Id = 11, NameLabel = "Тест_11" },
            new Label() { Id = 12, NameLabel = "Тест_12" },
            new Label() { Id = 13, NameLabel = "Тест_13" },
            new Label() { Id = 14, NameLabel = "Тест_14" },
            new Label() { Id = 15, NameLabel = "Тест_15" },
            new Label() { Id = 16, NameLabel = "Тест_16" },
            new Label() { Id = 17, NameLabel = "Тест_17" },
            new Label() { Id = 18, NameLabel = "Тест_18" },
            new Label() { Id = 19, NameLabel = "Тест_19" },
            new Label() { Id = 20, NameLabel = "Тест_20" },
            new Label() { Id = 21, NameLabel = "Тест_21" },
        };
    }

    public Task<bool> LoadDatabaseAsync(string path)
    {
        try
        {
            _logger.Debug("[MockupRepository:LoadDatabaseAsync]  call init");

            _allImagesPath = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
            Array.Sort(_allImagesPath);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[MockupRepository:LoadDatabase] {ErrorMessage}", ex.Message);
        }

        return Task.FromResult(false);
    }

    public Task<int[]> GetAllIndexImagesAsync()
    {
        var res = Task.FromResult(_allImagesPath?.Any() is false
            ? Array.Empty<int>()
            : _allImagesPath.Select((name, index) => index).ToArray());
        return res;
    }

    public Task<ImageFrame[]> GetAllImagesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Annotation[]> GetAllAnnotationsAsync()
    {
        throw new NotImplementedException();
    }


    public async Task<ImageFrame> GetImagesByIndexAsync(int idImages)
    {
        var img = Array.Empty<byte>();
        if (_allImagesPath?.Any() is false || idImages >= _allImagesPath.Length || idImages < 0)
        {
            _logger.Error("[MockupRepository:GetImagesByIndexAsync] Fail Find index {IdImages}; " +
                          "AllImg: {AllImg}", idImages,
                _allImagesPath.Length);
            img = await File.ReadAllBytesAsync("Resource/error_1.png");
        }
        else
        {
            _logger.Debug("[MockupRepository:GetImagesByIndexAsync] get img index {IdImages}", idImages);
            var pathImg = _allImagesPath[idImages];
            img = await File.ReadAllBytesAsync(pathImg);
        }


        var (width, height, resizeByte) = GetSizeImg(img);

        return new ImageFrame()
            { Id = idImages, 
                Images = resizeByte, 
                SizeImage = new SizeF() { Width = width, Height = height },
                NameImages = _allImagesPath[idImages]
            };
    }

    public Task<Label[]> GetAllLabelsAsync()
    {
        return Task.FromResult(_labels);
    }


    public Task<Annotation[]> GetAnnotationsFromImgIdAsync(int imagesId)
    {
       var res = _annotations.Where(p => p.ImageFrameId == imagesId).ToArray();
        return Task.FromResult(res);
    }

    public async Task<bool> DeleteAnnotationsAsync(Annotation[] removeAnnotations)
    {
        foreach (var removeAnnot in removeAnnotations)
        {
            var findOldAnnot = _annotations.FirstOrDefault(p => p.Id == removeAnnot.Id);
            if (findOldAnnot is null)
                continue;
            _annotations.Remove(findOldAnnot);
        }

        _logger.Debug("[DeleteAnnotationsAsync] Remove count: {Annotations} ", removeAnnotations.Length);
        return true;
    }

    public async Task<bool> SaveAnnotationsAsync(Annotation[] annotToSave)
    {
        if (annotToSave.Any() is false)
            return false;

        _annotations.AddRange(annotToSave);
        _logger.Debug("[SaveAnnotationsAsync] Save count All: {Annotations} ", _annotations.Count);
        return true;
    }

    public Task<int> GetLastIndexAnnotation()
    {
        var lastId = -1;
        if(_annotations.Any())
             lastId = _annotations.Max(p => p.Id);
        
        return Task.FromResult(lastId);
    }

    public Task<bool> InsertImageFrames(ImageFrame[]? frame)
    {
        throw new NotImplementedException();
    }

    public Task<bool> InsertLabels(Label? [] frame)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveInformationDtoAsync(InformationDto frame)
    {
        throw new NotImplementedException();
    }

    public Task<InformationDto[]> GetInformationDtoAsync()
    {
        throw new NotImplementedException();
    }


    private (int width, int height, byte[] img) GetSizeImg(byte[] img)
    {
        using var image = new MagickImage(img);

        image.Resize(1000, 800);
        image.Quality = 50;
        image.Format = MagickFormat.Jpg;

        var data = image.ToByteArray();

        var width = image.Width;
        var height = image.Height;
        return (width, height, data);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}