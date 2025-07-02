using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Инициализация базы данных.
/// </summary>
public class ChoseActiveDataBaseHandler:IRequestHandler<ChoseActiveDataBaseQueries,bool>
{
    private readonly ILogger _logger = Log.ForContext<ChoseActiveDataBaseHandler>();
    private readonly IRepository _repository;
    private readonly MarkupData _markupData;
    private readonly IMediator _mediator;

    public ChoseActiveDataBaseHandler(IRepository repository, MarkupData markupData, IMediator mediator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(ChoseActiveDataBaseQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null || string.IsNullOrWhiteSpace(request.PathDb))
                return false;

            var pathDb = request.PathDb;
            
            if (!File.Exists(pathDb))
                return false;
     
            var res = await _repository.LoadDatabaseAsync(pathDb);
            if (!res)
            {
                Log.Error("[HandlerChoseActiveDataBaseAsync] fail LoadDatabaseAsync {PathDb}", pathDb);
                return false;
            }

            var idImg = 1;
            _markupData.CurrentIdImg = idImg;
            _markupData.CurrentProgress = 0;
            var imageFrame = await _repository.GetImagesByIndexAsync(idImg);
            if (!imageFrame.Images.Any())
                return false;

            _markupData.ImagesUI = $"data:image/jpg;base64," + Convert.ToBase64String(imageFrame.Images);
            _markupData.SizeConvas = new SizeT()
            {
                Width = imageFrame.SizeImage.Width,
                Height = imageFrame.SizeImage.Height
            };
        
            await _mediator.Send(new LoadAnnotationsSlowStorageQueries() { ImageId = idImg }, cancellationToken);
            
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[ChoseActiveDataBaseHandler] {@Exception}", e);
        }
        return false;
    }
}