using System.Data;
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
                throw new InvalidOperationException("[ChoseActiveDataBaseHandler] Fail path DB");
     
            var res = await _repository.LoadDatabaseAsync(pathDb);
            if (!res)
            {
                throw new InvalidOperationException($"[ChoseActiveDataBaseHandler] fail LoadDatabaseAsync {pathDb}");
            }

            var idImg = 1;
            _markupData.CurrentIdImg = idImg;
            _markupData.CurrentProgress = 0;
            var resLoadImage =  await _mediator.Send(new LoadByIndexImageQueries() { IndexImage = idImg }, cancellationToken);
            if (!resLoadImage)
            {
                throw new InvalidOperationException($"[ChoseActiveDataBaseHandler] fail GetImagesByIndexAsync {pathDb}");
            }
   
        
            await _mediator.Send(new LoadAnnotationsSlowStorageQueries() { ImageId = idImg }, cancellationToken);

            var labelsDb = await _repository.GetAllLabelsAsync();
            if (labelsDb is null || labelsDb.Any() is false)
            {
                throw new InvalidOperationException($"[ChoseActiveDataBaseHandler]  fail load LabelsName");
            }
            _markupData.LabelsName = labelsDb;

            await SetAllImagesCount();

            InitNameDb(pathDb);
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[ChoseActiveDataBaseHandler] {@Exception}", e);
        }
        return false;
    }

    private void InitNameDb(string pathDb)
    {
        var name = Path.GetFileName(pathDb);
        _markupData.NameDb = name;
    }

    private async Task SetAllImagesCount()
    {
        var allIndex = await _repository.GetAllIndexImagesAsync();
        if (!allIndex.Any())
            return;

        _markupData.AllImagesCount = allIndex.Length;
    }
}