using BrowserInteractLabeler.Common.DTO;

namespace BrowserInteractLabeler.Common;

public interface IRepository : IDisposable
{
    public bool LoadDatabase(string path);

    public int[] GetAllIndexImages();
    
    ImageFrame[] GetAllImages();
    
    public Annotation[] GetAllAnnotations();

    public ImageFrame GetImagesByIndex(int imagesId);

    public Label[] GetAllLabels();

    public Annotation[] GetAnnotationsFromImgId(int imagesId);

    public bool DeleteAnnotations(Annotation[] removeAnnot);
    
    public bool SaveAnnotations(Annotation[] toArray);
    
    public int GetLastIndexAnnotation();

    public bool InsertImageFrames(ImageFrame[]? frame);

    public bool InsertLabels(Label?[]? frame);

    public bool SaveInformationDto(InformationDto frame);

    public InformationDto[] GetInformationDto();
}