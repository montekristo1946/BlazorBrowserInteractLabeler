using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BlazorBrowserInteractLabeler.Web.Properties;
using BrowserInteractLabeler.Common;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

public class UnloadDataBaseHandler : IRequestHandler<UnloadDataBaseQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<UnloadDataBaseHandler>();
    private readonly MarkupData _markupData;
    private readonly IRepository _repository;
    private readonly AnnotationHandler _annotationHandler;

    public UnloadDataBaseHandler(MarkupData markupData, IRepository repository, AnnotationHandler annotationHandler)
    {
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
    }

    public async Task<bool> Handle(UnloadDataBaseQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;

            await _repository.UnloadDatabaseAsync();
            await _annotationHandler.UpdateAllAnnotations([]);

            var srcImg = Resources.ImagesMoq;
            var base64 = $"data:image/jpg;base64," + Convert.ToBase64String(srcImg);
            _markupData.ImagesUI = base64;
            _markupData.SizeConvas = new SizeT()
            {
                Width = 1800,
                Height = 900,
            };
            _markupData.CrosshairData = new CrosshairData()
            {
                IsShowCrosshair = true,
                Color = "#ea0d0d"
            };

            _markupData.CurrentIdImg = 1;
            _markupData.CurrentProgress = 0;
            _markupData.LabelsName = [];

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[UnloadDataBaseHandler] {@Exception}", e);
        }


        return false;
    }
}