using BlazorBrowserInteractLabeler.ARM.Extension;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers;

public class AnnotationHandler
{
    private SemaphoreSlim _semaphoreSlim = new(1, 1);
    private List<Annotation> _annotations = new();
    private readonly int _timeWaitSeamaphore = 10;


    public async Task<Annotation[]> GetAllAnnotations()
    {
        await _semaphoreSlim.WaitAsync(_timeWaitSeamaphore);
        try
        {
            return _annotations.ToArray();
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


    public async Task UpdateAllAnnotations(Annotation[] allAnnots)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            _annotations = allAnnots.ToList();
        }
        catch (Exception e)
        {
            Log.Error("[EventEditAnnotForceCreateNew] {@Exception}", e);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}