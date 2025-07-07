using BrowserInteractLabeler.Common.DTO;

namespace BrowserInteractLabeler.Common;

public interface IRepository : IDisposable
{
    public Task<bool> LoadDatabaseAsync(string path);
    //
    public Task<int[]> GetAllIndexImagesAsync();
    //
    public Task<ImageFrame[]> GetInfoAllImagesAsync();

    public Task<Annotation[]> GetAllAnnotationsAsync();
    //
    public Task<ImageFrame> GetImagesByIndexAsync(int imagesId);

    public Task<Annotation[]> GetAnnotationsFromImgIdAsync(int imagesId);

    public Task<bool> DeleteAnnotationsAsync(Annotation[] removeAnnot);
    
    public Task<bool> SaveAnnotationsAsync(Annotation[] toArray);

    public Task<Label[]> GetAllLabelsAsync();

//     public int GetLastIndexAnnotationAsync();
//
//     public bool InsertImageFramesAsync(ImageFrame[]? frame);
//
    public Task<bool> InsertLabelsAsync(Label[] labels);
    public  Task<bool> InsertImageFramesAsync(ImageFrame[] frame);
//
//     public bool SaveInformationDtoAsync(InformationDto frame);
//
//     public InformationDto[] GetInformationDtoAsync();
}