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
    // private int _lastIdDb = -1;
    private readonly ILogger _logger = Log.ForContext<CacheAnnotation>();

    public CacheAnnotation(IRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<(bool checkResult, Annotation annot)> GetEditAnnotation()
    {
        var annot = _annotations.FirstOrDefault(p => p.State != StateAnnot.Finalized);
        if (annot is not null)
            return (true, annot);

        return (false, new Annotation());
    }

    public async Task UpdateAnnotation(Annotation annotation)
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


    public Annotation[] GetAllAnnotations(int imagesId)
    {
        return _annotations.Where(p => p.ImageFrameId == imagesId).ToArray();
    }

    public async Task SaveAnnotationsOnSqlAsync(int imagesId)
    {
        var removeAnnot = await _repository.GetAnnotationsFromImgIdAsync(imagesId);

        var equalAnnotation = removeAnnot.Equality(_annotations.ToArray());
        if (equalAnnotation)
            return;


        _logger.Debug("[SaveAnnotationsOnSqlAsync] " +
                      "Save annotations in Img:{imagesId} count annotations:{CountAnnotations}", imagesId,
            _annotations.Count);


        await _repository.DeleteAnnotationsAsync(removeAnnot);
        _annotations = ClearFailAnnotation(_annotations);
        await _repository.SaveAnnotationsAsync(_annotations.ToArray());

        var allAnnot = await _repository.GetAnnotationsFromImgIdAsync(imagesId);
        _annotations = allAnnot.CloneDeep().ToList();
    }

    private List<Annotation> ClearFailAnnotation(IEnumerable<Annotation> annotations)
    {
        if (annotations?.Any() == null)
            return new List<Annotation>();

        var retAnnots = annotations.Where(annot => annot.Points?.Any() != null)
            .Where(annot =>
                (annot.LabelPattern == TypeLabel.Box && annot.Points.Count > 1)
                || (annot.LabelPattern == TypeLabel.PolyLine && annot.Points.Count > 1)
                || (annot.LabelPattern == TypeLabel.Polygon && annot.Points.Count > 2)
                || (annot.LabelPattern == TypeLabel.Point && annot.Points.Count > 0)
            ).Select(annot =>
            {
                annot.State = StateAnnot.Finalized;
                annot.Id = 0;
                return annot;
            }).ToList();
        
        return retAnnots;
    }


    public void RemoveLastAnnotation(int imagesId)
    {
        var last = _annotations.LastOrDefault(p => p.ImageFrameId == imagesId);
        if (last is null)
            return;
        _lastAnnotation = last;
        _annotations.Remove(_lastAnnotation);
    }

    public void RestoreLastAnnotation(int imagesId)
    {
        if (_lastAnnotation.Id < 0 || _lastAnnotation.ImageFrameId != imagesId)
            return;

        _annotations.Add(_lastAnnotation);
        _lastAnnotation = new Annotation();
    }

    public async Task LoadAnnotationsSlowStorageAsync(int imagesId)
    {
        var allAnnot = await _repository.GetAnnotationsFromImgIdAsync(imagesId);
        _annotations = allAnnot.CloneDeep().ToList();

        // _lastIdDb = await _repository.GetLastIndexAnnotation();
    }

    public void DeleteAnnotation()
    {
        var last = _annotations.LastOrDefault(p => p.State != StateAnnot.Finalized);
        if (last is null)
            return;
        _lastAnnotation = last;
        _annotations.Remove(_lastAnnotation);
    }

    private void CreateNewAnnot(int imagesId, TypeLabel typeLabel = TypeLabel.None)
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

    /// <summary>
    ///     key q,w,a,s
    /// </summary>
    /// <param name="imagesId"></param>
    public void EventEditAnnotForceCreateNew(int imagesId, TypeLabel typeLabel = TypeLabel.None)
    {
        foreach (var annotation in _annotations)
        {
            annotation.State = StateAnnot.Finalized;
        }

        CreateNewAnnot(imagesId, typeLabel);
    }

    /// <summary>
    ///     key [e]
    /// </summary>
    /// <param name="imagesId"></param>
    public void EventEditAnnot(int imagesId)
    {
        var last = _annotations.LastOrDefault(p => p.State != StateAnnot.Finalized);
        if (last is not null)
        {
            last.State = StateAnnot.Finalized;
            return;
        }

        CreateNewAnnot(imagesId);
    }

    public (bool checkRes, Annotation annotation ) SetActiveAnnot(int idAnnot)
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

    public void SetActiveIdLabel(int id)
    {
        var current = _annotations.LastOrDefault(p => p.State != StateAnnot.Finalized);
        if (current is not null)
        {
            current.LabelId = id;
        }
    }
}