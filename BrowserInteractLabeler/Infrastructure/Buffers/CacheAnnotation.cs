using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;

namespace BrowserInteractLabeler.Infrastructure;

public class CacheAnnotation
{
    private readonly IRepository _repository;
    private Annotation _lastAnnotation = new();
    private List<Annotation> _annotations = new();
    private int _lastIdDb = -1;
    public CacheAnnotation(    IRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
       
    }
    
   public async Task<(bool result,Annotation annot)> GetEditAnnotation()
    {
     
        var annot = _annotations.FirstOrDefault(p => p.State!=StateAnnot.Finalized);
        if (annot is not null)
            return (true,annot);
        
        return (false,new Annotation());
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


    public async Task<Annotation[]> GetAllAnnotations(int imagesId)
    {
        return _annotations.Where(p=>p.ImageFrameId==imagesId).ToArray();
    }

    public async Task SaveAnnotationsOnSql(int imagesId)
    {
        var removeAnnot =  await _repository.GetAnnotationsFromImgIdAsync(imagesId);
        await _repository.DeleteAnnotationsAsync(removeAnnot);
        foreach (var annotation in _annotations)
        {
            annotation.State = StateAnnot.Finalized;
        }
   
        await _repository.SaveAnnotationsAsync(_annotations.ToArray());
    }

    public async Task RemoveLastAnnotation(int imagesId)
    {
        var last = _annotations.LastOrDefault(p => p.ImageFrameId==imagesId);
        if (last is null)
            return;
        _lastAnnotation = last;
        _annotations.Remove(_lastAnnotation);

    }

    public async Task RestoreLastAnnotation(int imagesId)
    {
        if(_lastAnnotation.Id<0 || _lastAnnotation.ImageFrameId !=imagesId )
            return;
        
        _annotations.Add(_lastAnnotation);
        _lastAnnotation = new Annotation();
    }

    public async Task LoadAnnotationsSlowStorage(int imagesId)
    {
        var allAnnot =  await _repository.GetAnnotationsFromImgIdAsync(imagesId);
        _annotations =allAnnot.ToList();
    
        _lastIdDb = await _repository.GetLastIndexAnnotation();
 
    }

    public async Task DeleteAnnotation()
    {
        var last = _annotations.LastOrDefault(p => p.State!=StateAnnot.Finalized);
        if (last is null)
            return;
        _lastAnnotation = last;
        _annotations.Remove(_lastAnnotation);
    }

    public async Task EventEditAnnot(int imagesId)
    {
        var last = _annotations.LastOrDefault(p => p.State!=StateAnnot.Finalized);
        if (last is not null)
        {
            last.State = StateAnnot.Finalized;
            return;
        }

        _lastIdDb += 1;
        var annot =new Annotation()
        {
            Id = _lastIdDb,
            Points = new List<PointF>(),
            ImageFrameId = imagesId,
            State = StateAnnot.Edit,
            LabelId = -1
        };
        _annotations.Add(annot);
    }

    public async Task<bool> SetActiveAnnot(int idAnnot)
    {
        var current = _annotations.LastOrDefault(p => p.Id==idAnnot);
     

        if (current is not null)
        {
            foreach (var annotation in _annotations)
            {
                annotation.State = StateAnnot.Finalized;
            }
            current.State = StateAnnot.Active;
            return true;
        }

        return false;
    }

    public async Task SetActiveIdLabel(int id)
    {
        var current = _annotations.LastOrDefault(p => p.State!=StateAnnot.Finalized);
        if (current is not null)
        {
            current.LabelId = id;
        }
    }
}