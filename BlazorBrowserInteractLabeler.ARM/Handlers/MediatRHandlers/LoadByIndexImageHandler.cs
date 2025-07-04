using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Загрузить по индексу изображение из базы данных.
/// </summary>
public class LoadByIndexImageHandler : IRequestHandler<LoadByIndexImageQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<LoadByIndexImageHandler>();
    private readonly MarkupData _markupData;
    private readonly IRepository _repository;
    private readonly AnnotationHandler _annotationHandler;
    private readonly IMediator _mediator;

    public LoadByIndexImageHandler(MarkupData markupData, IRepository repository, AnnotationHandler annotationHandler,
        IMediator mediator)
    {
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(LoadByIndexImageQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null )
                return false;

            var newIndex = request.IndexImage;
            var allIndex = await _repository.GetAllIndexImagesAsync();

            if (newIndex > allIndex.Length || newIndex < 1)
            {
                _logger.Error("[LoadByIndexImageHandler] the list is over {IndexImage} all:{AllCountImg}", newIndex, allIndex.Length);
                return false;
            }

            
            await _mediator.Send(new SaveAnnotationsOnSlowStorageQueries() , cancellationToken);
            _markupData.CurrentIdImg = newIndex;
            SetCurrentProgress();
            await LoadImage(newIndex);
            await _mediator.Send(new LoadAnnotationsSlowStorageQueries() { ImageId = newIndex }, cancellationToken);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[LoadByIndexImageHandler] {@Exception}", e);
        }

        return false;
    }


    private async Task  LoadImage(int index)
    {
        var imageFrame = await _repository.GetImagesByIndexAsync(index);
        if (!imageFrame.Images.Any())
        {
            throw new InvalidOperationException($"[LoadByIndexImageHandler] not Load Images from index {index}");
        }

        _markupData.ImagesUI = $"data:image/jpg;base64," + Convert.ToBase64String(imageFrame.Images);
        _markupData.SizeConvas = new SizeT()
        {
            Width = imageFrame.SizeImage.Width,
            Height = imageFrame.SizeImage.Height
        };
    }

    private void SetCurrentProgress()
    {
        var currentProgress = 0.0;
        var allImages = (double)_markupData.AllImagesCount;
        if (allImages > 0)
        {
            currentProgress = _markupData.CurrentIdImg / allImages * 100;
        }

        _markupData.CurrentProgress = (int)currentProgress;
    }
}