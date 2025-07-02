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
public class LoadNextImageHandler:IRequestHandler<LoadNextImageQueries,bool>
{
    private readonly ILogger _logger = Log.ForContext<LoadNextImageHandler>();
    private readonly MarkupData _markupData;
    private readonly IRepository _repository;
    private readonly AnnotationHandler _annotationHandler;
    private readonly IMediator _mediator;

    public LoadNextImageHandler(MarkupData markupData, IRepository repository, AnnotationHandler annotationHandler, IMediator mediator)
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
        
        await _mediator.Send(new SaveAnnotationsOnSlowStorageQueries() { ImageId = _markupData.CurrentIdImg });
        
        if (index > allIndex.Length || index < 1)
        {
            _logger.Debug("[HandlerChoseActiveDataBaseAsync] the list is over");
            return;
        }

        _markupData.CurrentIdImg = index;
        
        var imageFrame = await _repository.GetImagesByIndexAsync(index);
        if (!imageFrame.Images.Any())
            return;
        
        _markupData.ImagesUI = $"data:image/jpg;base64," + Convert.ToBase64String(imageFrame.Images);
        _markupData.SizeConvas = new SizeT()
        {
            Width = imageFrame.SizeImage.Width,
            Height = imageFrame.SizeImage.Height
        };
        
        await _mediator.Send(new LoadAnnotationsSlowStorageQueries() { ImageId = index });
    }
}