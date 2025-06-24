using BlazorBrowserInteractLabeler.ARM.Extension;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers;

public class AnnotationHandler
{
    private readonly IRepository _repository;
    private SemaphoreSlim _semaphoreSlim = new (1, 1);
    private List<Annotation> _annotations = new();
    private readonly int _timeWaitSeamaphore = 10;

    public AnnotationHandler(IRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

  
    public async Task LoadAnnotationsSlowStorageAsync(int imagesId)
    {
        await _semaphoreSlim.WaitAsync(_timeWaitSeamaphore);
        try
        {
            var allAnnots = _repository.GetAnnotationsFromImgId(imagesId);
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
                .ToList();

            _annotations = annotations;
        }
        catch (Exception e)
        {
            Log.Error("[LoadAnnotationsSlowStorageAsync] {@Exception}", e);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
    
    private float CalculateArea(List<PointF>? argPoints)
    {
        if (argPoints is null)
            return 0;

        var minX = argPoints.Min(p => p.X);
        var maxX = argPoints.Max(p => p.X);
        var minY = argPoints.Min(p => p.Y);
        var maxY = argPoints.Max(p => p.Y);
        return (maxX - minX) * (maxY - minY);
    }

    public async Task<Annotation[]> GetAllAnnotationsOnImg(int imagesId)
    {
       await _semaphoreSlim.WaitAsync(_timeWaitSeamaphore);
        try
        {
            _annotations = _annotations.DistinctBy(p => p.Id).ToList();//clear duplicate
            var res =_annotations.Where(p => p.ImageFrameId == imagesId)
                .ToArray();
            return res;
        }
        catch (Exception e)
        {
            Log.Error("[GetAllAnnotations] {@Exception}", e);
        }
        finally
        {
            _semaphoreSlim.Release();
        }

        return [];
    }
    
    public async Task SaveAnnotationsOnSqlAsync(int imagesId)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            var removeAnnot = _repository.GetAnnotationsFromImgId(imagesId);
            var annotations = _annotations.CloneDeep();
            var equalAnnotation = removeAnnot.Equality(annotations);
            if (equalAnnotation)
                return;

            Log.Debug("[SaveAnnotationsOnSqlAsync] " +
                          "Save annotations in Img:{imagesId} count annotations:{CountAnnotations}", imagesId,
                annotations.Count());


            _repository.DeleteAnnotations(removeAnnot);
            annotations = ClearFailAnnotation(annotations);
            annotations = OrderPoints(annotations);
            _repository.SaveAnnotations(annotations);

            var allAnnot = _repository.GetAnnotationsFromImgId(imagesId);
            _annotations = allAnnot.CloneDeep().ToList();
        }
        catch (Exception e)
        {
            Log.Error("[SaveAnnotationsOnSqlAsync] {@Exception}", e.Message);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
    private Annotation[] ClearFailAnnotation(Annotation[] annotations)
    {
        if (annotations?.Any() == null)
            return [];

        var retAnnots = annotations.Where(annot => annot.Points?.Any() != null)
            .Where(annot =>
                (annot.LabelPattern == TypeLabel.Box && annot.Points.Count > 1)
                || (annot.LabelPattern == TypeLabel.PolyLine && annot.Points.Count > 1)
                || (annot.LabelPattern == TypeLabel.Polygon && annot.Points.Count >= 3)
                || (annot.LabelPattern == TypeLabel.Point && annot.Points.Count > 0)
            ).Select(annot =>
            {
                annot.State = StateAnnot.Finalized;
                annot.Id = 0;
                return annot;
            }).ToArray();

        return retAnnots;
    }
    
    private Annotation[] OrderPoints(Annotation[] annotations)
    {
        if (annotations?.Any() == null)
            return [];

        var retArr = annotations.Select(annot =>
        {
            var newPoints = annot.Points
                ?.Select((point, index) => point with { Id = 0, PositionInGroup = index })
                .ToList();
            return annot with { Points = newPoints };
        }).ToArray();

        return retArr;
    }
}