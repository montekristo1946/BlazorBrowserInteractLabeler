using BlazorBrowserInteractLabeler.ARM.Extension;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Загрузить Annots из базы данных.
/// </summary>
public class LoadAnnotationsSlowStorageHandler : IRequestHandler<LoadAnnotationsSlowStorageQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<LoadNextImageHandler>();
    private readonly IRepository _repository;
    private readonly MarkupData _markupData;
    private readonly AnnotationHandler _annotationHandler;

    public LoadAnnotationsSlowStorageHandler(IRepository repository, MarkupData markupData, AnnotationHandler annotationHandler)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
    }

    public async Task<bool> Handle(LoadAnnotationsSlowStorageQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;

            var indexImg = _markupData.CurrentIdImg;

            var allAnnots = await _repository.GetAnnotationsFromImgIdAsync(indexImg);
            var cloneAnnots = allAnnots.CloneDeep().ToList();
            var annotations = cloneAnnots.Select(annot =>
                {
                    if (annot.Points?.Any() == false)
                        return annot;

                    var checkRestore = annot.Points.Count(p => p.PositionInGroup == -1);
                    if (checkRestore == annot.Points.Count()) //restoration position
                        annot.Points = annot.Points.Select((p, i) => p with { PositionInGroup = i }).ToList();

                    return annot;
                })
                .OrderBy(p => p.LabelId)
                .ThenByDescending(p => CalculateArea(p.Points))
                .ToArray();

            await _annotationHandler.UpdateAllAnnotations(annotations);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[LoadNextImageHandler] {@Exception}", e);
        }

        return false;
    }

    private double CalculateArea(List<PointD>? argPoints)
    {
        if (argPoints is null)
            return 0;

        var minX = argPoints.Min(p => p.X);
        var maxX = argPoints.Max(p => p.X);
        var minY = argPoints.Min(p => p.Y);
        var maxY = argPoints.Max(p => p.Y);
        return (maxX - minX) * (maxY - minY);
    }
}