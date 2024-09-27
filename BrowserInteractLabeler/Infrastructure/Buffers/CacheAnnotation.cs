using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Serilog;
using ILogger = Serilog.ILogger;

namespace BrowserInteractLabeler.Infrastructure;

public class CacheAnnotation
{
    private readonly IRepository _repository;
    private Annotation _lastAnnotation = new();
    private List<Annotation> _annotations = new();
    private readonly ILogger _logger = Log.ForContext<CacheAnnotation>();
    private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    public CacheAnnotation(IRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public (bool checkResult, Annotation annot) GetEditAnnotation()
    {
        semaphoreSlim.Wait();
        try
        {
            var annot = _annotations.FirstOrDefault(p =>
                p.State != StateAnnot.Finalized && p.State != StateAnnot.Hidden);

            if (annot is not null)
                return (true, annot.CloneDeep());

            return (false, new Annotation());
        }
        catch (Exception e)
        {
            _logger.Error("[GetEditAnnotation] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }

        return (false, new Annotation());
    }

    public void UpdateAnnotation(Annotation annotation)
    {
        semaphoreSlim.Wait();
        try
        {
            var currentAnnot = _annotations.FirstOrDefault(p => p.Id == annotation.Id);
            if (currentAnnot is null)
                _annotations.Add(annotation);
            else
            {
                _annotations.Remove(currentAnnot);
                _annotations.Add(annotation);
            }
        }
        catch (Exception e)
        {
            _logger.Error("[UpdateAnnotation] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private List<Annotation> CleanDuplicateIDAnnotations(List<Annotation> annotations)
    {
        var retArr = annotations.DistinctBy(p => p.Id).ToList();
        return retArr;
    }

    public Annotation[] GetAllAnnotationsOnImg(int imagesId)
    {
        semaphoreSlim.Wait();
        try
        {
            _annotations = CleanDuplicateIDAnnotations(_annotations);

            return _annotations.Where(p => p.ImageFrameId == imagesId).ToArray();
        }
        catch (Exception e)
        {
            _logger.Error("[GetAllAnnotations] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }

        return Array.Empty<Annotation>();
    }


    public async Task SaveAnnotationsOnSqlAsync(int imagesId)
    {
        await semaphoreSlim.WaitAsync();
        try
        {
            var removeAnnot = _repository.GetAnnotationsFromImgId(imagesId);
            var annotations = _annotations.CloneDeep();
            var equalAnnotation = removeAnnot.Equality(annotations);
            if (equalAnnotation)
                return;

            _logger.Debug("[SaveAnnotationsOnSqlAsync] " +
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
            _logger.Error("[SaveAnnotationsOnSqlAsync] {@Exception}", e.Message);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private Annotation[] OrderPoints(Annotation[] annotations)
    {
        if (annotations?.Any() == null)
            return Array.Empty<Annotation>();

        var retArr = annotations.Select(annot =>
        {
            var newPoints = annot.Points
                ?.Select((point, index) => point with { Id = 0, PositionInGroup = index })
                .ToList();
            return annot with { Points = newPoints };
        }).ToArray();

        return retArr;
    }

    private Annotation[] ClearFailAnnotation(Annotation[] annotations)
    {
        if (annotations?.Any() == null)
            return Array.Empty<Annotation>();

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

    public bool RemoveLastAnnotation(int imagesId)
    {
        semaphoreSlim.Wait();
        try
        {
            var last = _annotations.LastOrDefault(p => p.ImageFrameId == imagesId);
            if (last is null)
                return false;
            _lastAnnotation = last;
            _annotations.Remove(_lastAnnotation);
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[RemoveLastAnnotation] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }

        return false;
    }

    public bool RestoreLastAnnotation(int imagesId)
    {
        semaphoreSlim.Wait();
        try
        {
            if (_lastAnnotation.Id < 0 || _lastAnnotation.ImageFrameId != imagesId)
                return false;

            _annotations.Add(_lastAnnotation);
            _lastAnnotation = new Annotation();
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[RestoreLastAnnotation] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }

        return false;
    }

    public async Task LoadAnnotationsSlowStorageAsync(int imagesId)
    {
        await semaphoreSlim.WaitAsync();
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
            }).OrderBy(p => p.LabelId).ToList();

            _annotations = annotations;
        }
        catch (Exception e)
        {
            _logger.Error("[LoadAnnotationsSlowStorageAsync] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public void DeleteAnnotation()
    {
        semaphoreSlim.Wait();
        try
        {
            var last = _annotations.LastOrDefault(p => p.State != StateAnnot.Finalized);
            if (last is null)
                return;
            _lastAnnotation = last;
            _annotations.Remove(_lastAnnotation);
        }
        catch (Exception e)
        {
            _logger.Error("[DeleteAnnotation] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private void CreateNewAnnot(int imagesId, TypeLabel typeLabel = TypeLabel.None)
    {
        try
        {
            var lastAnnot = _annotations.MaxBy(p => p.Id);
            var currentDb = 1;

            if (lastAnnot is not null)
                currentDb = lastAnnot.Id + 1;

            var annot = new Annotation()
            {
                Id = currentDb,
                Points = new List<PointF>(),
                ImageFrameId = imagesId,
                State = StateAnnot.Edit,
                LabelId = -1,
                LabelPattern = typeLabel
            };
            _annotations.Add(annot);
        }
        catch (Exception e)
        {
            _logger.Error("[CreateNewAnnot] {@Exception}", e);
        }
    }

    /// <summary>
    ///     key q,w,a,s
    /// </summary>
    /// <param name="imagesId"></param>
    public void EventEditAnnotForceCreateNew(int imagesId, TypeLabel typeLabel = TypeLabel.None)
    {
        semaphoreSlim.Wait();
        try
        {
            foreach (var annotation in _annotations.Where(annotation => annotation.State != StateAnnot.Hidden))
            {
                annotation.State = StateAnnot.Finalized;
            }

            CreateNewAnnot(imagesId, typeLabel);
        }
        catch (Exception e)
        {
            _logger.Error("[EventEditAnnotForceCreateNew] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    /// <summary>
    ///     key [e]
    /// </summary>
    /// <param name="imagesId"></param>
    public void EventEditAnnot(int imagesId)
    {
        semaphoreSlim.Wait();
        try
        {
            var last = _annotations.LastOrDefault(p => p.State != StateAnnot.Finalized);
            if (last is not null)
            {
                last.State = StateAnnot.Finalized;
                return;
            }

            CreateNewAnnot(imagesId);
        }
        catch (Exception e)
        {
            _logger.Error("[EventEditAnnot] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public (bool checkRes, Annotation annotation) SetActiveAnnot(int idAnnot)
    {
        semaphoreSlim.Wait();
        try
        {
            var current = _annotations.LastOrDefault(p => p.Id == idAnnot);

            if (current is null)
                return (false, new Annotation());


            foreach (var annotation in _annotations)
            {
                annotation.State = StateAnnot.Finalized;
            }

            current.State = StateAnnot.Active;

            return (true, current);
        }
        catch (Exception e)
        {
            _logger.Error("[SetActiveAnnot] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }

        return (false, new Annotation());
    }

    public void SetActiveIdLabel(int id)
    {
        semaphoreSlim.Wait();
        try
        {
            var current = _annotations.LastOrDefault(p => p.State != StateAnnot.Finalized);
            if (current is not null)
            {
                current.LabelId = id;
            }
        }
        catch (Exception e)
        {
            _logger.Error("[GetAllAnnotations] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }


    public (bool checkRes, Annotation annotation) SetHiddenAnnot(int idAnnot)
    {

        semaphoreSlim.Wait();
        try
        {
            var current = _annotations.LastOrDefault(p => p.Id == idAnnot);

            if (current is null)
                return (false, new Annotation());

            foreach (var annotation in _annotations)
            {
                annotation.State = StateAnnot.Finalized;
            }

            current.State = current.State != StateAnnot.Hidden ? StateAnnot.Hidden : StateAnnot.Active;

            return (true, current);
        }
        catch (Exception e)
        {
            _logger.Error("[SetHiddenAnnot] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }

        return (false, new Annotation());
    }

    public bool SetHiddenAllAnnot()
    {
        semaphoreSlim.Wait();
        try
        {
            foreach (var annotation in _annotations)
            {
                annotation.State = StateAnnot.Hidden;
            }

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[SetHiddenAllAnnot] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }

        return false;
    }

    public bool SetFinalizeAllAnnot()
    {
        semaphoreSlim.Wait();
        try
        {
            foreach (var annotation in _annotations)
            {
                annotation.State = StateAnnot.Finalized;
            }

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[SetHiddenAllAnnot] {@Exception}", e);
        }
        finally
        {
            semaphoreSlim.Release();
        }

        return false;
    }
}