using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers;

public class ProjectsLocalHandler
{
    private readonly IRepository _repository;
    private readonly MarkupData _markupData;
    private readonly AnnotationHandler _annotationHandler;
    public event Action NeedUpdateUi;

    public ProjectsLocalHandler(IRepository repository, MarkupData markupData, AnnotationHandler annotationHandler)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
    }

    public void LoadBackImage()
    {
        var current = _markupData.CurrentIdImg;
        HandlerLoadImage(current - 1);
    }

    public void LoadNextImage()
    {
        var current = _markupData.CurrentIdImg;
        HandlerLoadImage(current + 1);
    }
    

    public async Task HandlerChoseActiveDataBaseAsync(string? pathDb)
    {
        if (string.IsNullOrWhiteSpace(pathDb))
            return;

        if (!File.Exists(pathDb))
            return;

        var res = _repository.LoadDatabase(pathDb);
        if (!res)
        {
            Log.Error("[HandlerChoseActiveDataBaseAsync] fail LoadDatabaseAsync {PathDb}\"", pathDb);
            return;
        }

        var idImg = 1;
        _markupData.CurrentIdImg = idImg;
        _markupData.CurrentProgress = 0;
        var imageFrame = _repository.GetImagesByIndex(idImg);
        if (!imageFrame.Images.Any())
            return;

        _markupData.ImagesUI = $"data:image/jpg;base64," + Convert.ToBase64String(imageFrame.Images);
        _markupData.SizeConvas = new SizeT()
        {
            Width = imageFrame.SizeImage.Width,
            Height = imageFrame.SizeImage.Height
        };
        await _annotationHandler.LoadAnnotationsSlowStorageAsync(idImg);

    }

    private async Task HandlerLoadImage(int index)
    {
        var allIndex = _repository.GetAllIndexImages();

        if (index > allIndex.Length || index < 1)
        {
            Log.Debug("[HandlerChoseActiveDataBaseAsync] the list is over");
            return;
        }

        await _annotationHandler.SaveAnnotationsOnSqlAsync(_markupData.CurrentIdImg);
        _markupData.CurrentIdImg = index;
        
        var imageFrame = _repository.GetImagesByIndex(index);
        if (!imageFrame.Images.Any())
            return;
        
        _markupData.ImagesUI = $"data:image/jpg;base64," + Convert.ToBase64String(imageFrame.Images);
        _markupData.SizeConvas = new SizeT()
        {
            Width = imageFrame.SizeImage.Width,
            Height = imageFrame.SizeImage.Height
        };
        
        await _annotationHandler.LoadAnnotationsSlowStorageAsync(index);
        
        NeedUpdateUi?.Invoke();
    }

  
}