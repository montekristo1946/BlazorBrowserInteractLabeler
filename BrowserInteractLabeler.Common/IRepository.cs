using BrowserInteractLabeler.Common.DTO;

namespace BrowserInteractLabeler.Common;

public interface IRepository: IDisposable
{
     public Task<bool> LoadDatabaseAsync(string path);

    public Task <int[]> GetAllIndexImagesAsync();

    public Task<ImageFrame[]> GetAllImagesAsync();
    public Task<Annotation[]> GetAllAnnotationsAsync();
    
    public Task <ImageFrame> GetImagesByIndexAsync(int imagesId);
    
    public Task<Label[]> GetAllLabelsAsync();

    public Task<Annotation[]> GetAnnotationsFromImgIdAsync(int imagesId);

    public Task<bool> DeleteAnnotationsAsync(Annotation[] removeAnnot);
    public Task<bool> SaveAnnotationsAsync(Annotation[] toArray);
    public Task<int> GetLastIndexAnnotation();

    public Task<bool> InsertImageFrames(ImageFrame[]? frame);

    public Task<bool> InsertLabels(Label?[]? frame);
    
    public Task<bool> SaveInformationDtoAsync(InformationDto frame);

    public Task<InformationDto[]> GetInformationDtoAsync();
}

