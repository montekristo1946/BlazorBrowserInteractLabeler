using BrowserInteractLabeler.Common.DTO;

namespace BrowserInteractLabeler.Common;

public interface IRepository : IDisposable
{
    public Task<bool> LoadDatabaseAsync(string path);
    //
    public Task<int[]> GetAllIndexImagesAsync();
    //
    // ImageFrame[] GetAllImagesAsync();
    //
    // public Annotation[] GetAllAnnotationsAsync();
    //
    public Task<ImageFrame> GetImagesByIndexAsync(int imagesId);
    //
    // public Label[] GetAllLabelsAsync();

    public Task<Annotation[]> GetAnnotationsFromImgIdAsync(int imagesId);

    public Task<bool> DeleteAnnotationsAsync(Annotation[] removeAnnot);
    
    public Task<bool> SaveAnnotationsAsync(Annotation[] toArray);
    
//     public int GetLastIndexAnnotationAsync();
//
//     public bool InsertImageFramesAsync(ImageFrame[]? frame);
//
//     public bool InsertLabelsAsync(Label?[]? frame);
//
//     public bool SaveInformationDtoAsync(InformationDto frame);
//
//     public InformationDto[] GetInformationDtoAsync();
}