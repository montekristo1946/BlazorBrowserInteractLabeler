using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Загрузить изображение из базы данных.
/// </summary>
public class LoadNextImageHandler : IRequestHandler<LoadNextImageQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<LoadNextImageHandler>();
    private readonly MarkupData _markupData;
    private readonly IRepository _repository;
    private readonly AnnotationHandler _annotationHandler;
    private readonly IMediator _mediator;

    public LoadNextImageHandler(MarkupData markupData, IRepository repository, AnnotationHandler annotationHandler,
        IMediator mediator)
    {
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(LoadNextImageQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;

            var current = _markupData.CurrentIdImg;
            if (request.IsForward)
            {
                current += 1;
            }
            else
            {
                current -= 1;
            }

            await HandlerLoadImage(current);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[LoadNextImageHandler] {@Exception}", e);
        }

        return false;
    }

    private async Task HandlerLoadImage(int index)
    {
        var allIndex = await _repository.GetAllIndexImagesAsync();

        await _mediator.Send(new SaveAnnotationsOnSlowStorageQueries() );

        if (index > allIndex.Length || index < 1)
        {
            _logger.Error("[LoadNextImageHandler] the list is over {IndexImage} all:{AllCountImg}", index,
                allIndex.Length);
            return;
        }

        _markupData.CurrentIdImg = index;
        SetCurrentProgress();

        var imageFrame = await _repository.GetImagesByIndexAsync(index);
        if (!imageFrame.Images.Any())
        {
            _logger.Error("[LoadNextImageHandler] not Load Images from index {IndexImage}", index);
            return;
        }

        _markupData.ImagesUI = $"data:image/jpg;base64," + Convert.ToBase64String(imageFrame.Images);
        _markupData.SizeConvas = new SizeT()
        {
            Width = imageFrame.SizeImage.Width,
            Height = imageFrame.SizeImage.Height
        };
        _markupData.NameImage= imageFrame.NameImages;


        await _mediator.Send(new LoadAnnotationsSlowStorageQueries() { ImageId = index });
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