using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BrowserInteractLabeler.Repository;

public class SqlRepository : IRepository
{
    private ApplicationDbContext? _db = null!;
    private readonly ILogger _logger = Log.ForContext<SqlRepository>();
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);

    public async Task<bool> LoadDatabaseAsync(string pathDb)
    {
        await SemaphoreSlim.WaitAsync();
        try
        {
            if (_db is not null)
            {
                await _db.DisposeAsync();
                _db = null;
            }

            _logger.Debug("[LoadDatabaseAsync] Init  {PathDb}", pathDb);
            var databaseConnectionString = $@"Data Source={pathDb};foreign keys=true;";
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(databaseConnectionString)
                .Options;

            _db = new ApplicationDbContext(contextOptions);

            _db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            _logger.Debug("[LoadDatabaseAsync] Load ok {PathDb}", pathDb);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[LoadDatabaseAsync] {@Exception}", e.Message);
        }
        finally
        {
            SemaphoreSlim.Release();
        }


        return false;
    }

    public async Task<int[]> GetAllIndexImagesAsync()
    {
        if (_db is null)
            return [];

        await SemaphoreSlim.WaitAsync();
        try
        {
            var retArr = _db.ImageFrames.Select(p => p.Id).ToArray();
            return retArr;
        }
        catch (Exception e)
        {
            _logger.Error("[GetAllIndexImagesAsync] {@Exception}", e.Message);
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        return [];
    }

    public async Task<ImageFrame> GetImagesByIndexAsync(int imagesId)
    {
        if (_db is null)
            return new ImageFrame();

        await SemaphoreSlim.WaitAsync();
        try
        {
            var res = _db.ImageFrames
                .Include(p => p.SizeImage)
                .FirstOrDefault(i => i.Id == imagesId);
            return res ?? new ImageFrame();
        }
        catch (Exception e)
        {
            _logger.Error("[GetImagesByIndexAsync] {@Exception}", e.Message);
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        return new ImageFrame();
    }

    public async Task<Annotation[]> GetAnnotationsFromImgIdAsync(int imagesId)
    {
        if (_db is null)
            return [];

        await SemaphoreSlim.WaitAsync();
        try
        {
            var annotations = _db.Annotations
                .Include(point => point.Points)
                .Where(p => p.ImageFrameId == imagesId)
                .ToArray();

            return annotations;
        }
        catch (Exception e)
        {
            _logger.Error("[GetAnnotationsFromImgIdAsync] {@Exception}", e.Message);
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        return [];
    }

    public async Task<bool> DeleteAnnotationsAsync(Annotation[]? removeAnnot)
    {
        if (_db is null || removeAnnot is null || !removeAnnot.Any())
            return false;

        await SemaphoreSlim.WaitAsync();
        try
        {
            var removeId = removeAnnot.Select(annot => annot.Id).ToArray();
            await _db.Annotations
                .Where(p => removeId.Contains(p.Id))
                .ExecuteDeleteAsync();

            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[DeleteAnnotationsAsync] {@Exception}", e.Message);
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        return false;
    }

    public async Task<bool> SaveAnnotationsAsync(Annotation[]? annotations)
    {
        if (_db is null || annotations is null || !annotations.Any())
            return false;


        await SemaphoreSlim.WaitAsync();
        try
        {
            await _db.Annotations.AddRangeAsync(annotations);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[SaveAnnotationsAsync] {@Exception}", e.Message);
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        return false;
    }

    public async Task<Label[]> GetAllLabelsAsync()
    {
        if (_db is null)
            return [];


        await SemaphoreSlim.WaitAsync();
        try
        {
            var labels = _db.Labels.ToArray();
            return labels;
        }
        catch (Exception e)
        {
            _logger.Error("[GetAllLabelsAsync] {@Exception}", e.Message);
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        return [];
    }

    public async Task<ImageFrame[]> GetInfoAllImagesAsync()
    {
        if (_db is null)
            return [];


        await SemaphoreSlim.WaitAsync();
        try
        {
            var retArr = _db.ImageFrames.Select(s => new
                {
                    s.Id, s.NameImages
                })
                .Select(p => new ImageFrame()
                {
                    Id = p.Id,
                    NameImages = p.NameImages
                }).ToArray();

            return retArr;
        }
        catch (Exception e)
        {
            _logger.Error("[GetAllImagesAsync] {@Exception}", e.Message);
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        return [];
    }

    public async Task<Annotation[]> GetAllAnnotationsAsync()
    {
        if (_db is null)
            return [];


        await SemaphoreSlim.WaitAsync();
        try
        {
            var retArr = _db.Annotations
                .Include(p => p.Points)
                .ToArray();
            return retArr;
        }
        catch (Exception e)
        {
            _logger.Error("[GetAllAnnotations] {@Exception}", e.Message);
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        return [];
    }

    public async Task<bool> InsertLabelsAsync(Label[]? labels)
    {
        if (_db is null)
            return false;


        await SemaphoreSlim.WaitAsync();
        try
        {
            if (labels is null || !labels.Any())
                return false;
            
            await _db.Labels.AddRangeAsync(labels!);
            await _db.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[InsertLabelsAsync] {@Exception}", e.Message);
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        return false;
    }
    public  async Task<bool> InsertImageFramesAsync(ImageFrame[]? frame)
    {
        if (_db is null)
            return false;

        await SemaphoreSlim.WaitAsync();
        try
        {
            if (frame is null || !frame.Any())
                return false;
            
            await _db.ImageFrames.AddRangeAsync(frame);
            await _db.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[InsertImageFramesAsync] {@Exception}", e.Message);
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        return false;
    }
    
    
    // public bool InsertLabels(Label?[]? labels)
    // {
    //     lock (_locker)
    //     {
    //         if (labels is null || !labels.Any() || _db is null)
    //             return false;
    //
    //         _db.Labels.AddRangeAsync(labels!);
    //         _db.SaveChangesAsync();
    //
    //         return true;
    //     }
    // }
    public void Dispose()
    {
        _db?.Dispose();
    }
    //
    // public int[] GetAllIndexImages()
    // {
    //     if (_db is null)
    //         return [];
    //
    //     lock (_locker)
    //     {
    //         var retArr = _db.ImageFrames.AsNoTracking().Select(p => p.Id).ToArray();
    //         return retArr;
    //     }
    // }
    //

    //

    //
    // public ImageFrame GetImagesByIndex(int imagesId)
    // {
    //     if (_db is null)
    //         return new ImageFrame();
    //
    //     lock (_locker)
    //     {
    //         var res = _db.ImageFrames
    //             .Include(p => p.SizeImage)
    //             .AsNoTracking()
    //             .FirstOrDefault(i => i.Id == imagesId);
    //         return res ?? new ImageFrame();
    //     }
    // }
    //
    // public Label[] GetAllLabels()
    // {
    //     if (_db is null)
    //         return [];
    //
    //     lock (_locker)
    //     {
    //         var labels = _db.Labels.AsNoTracking().ToArray();
    //         return labels;
    //     }
    // }
    //
    // public Annotation[] GetAnnotationsFromImgId(int imagesId)
    // {
    //     if (_db is null)
    //         return [];
    //     lock (_locker)
    //     {
    //         var annotations = _db.Annotations
    //             .Include(point => point.Points)
    //             .Where(p => p.ImageFrameId == imagesId)
    //             .AsNoTracking()
    //             .ToArray();
    //
    //         return annotations;
    //     }
    // }
    //
    //
    // public bool DeleteAnnotations(Annotation[]? removeAnnot)
    // {
    //     if (_db is null)
    //         return false;
    //
    //     lock (_locker)
    //     {
    //         if (removeAnnot is null || !removeAnnot.Any())
    //             return false;
    //
    //         var removeId = removeAnnot.Select(annot => annot.Id).ToArray();
    //         _db.Annotations
    //             .Where(p => removeId.Contains(p.Id))
    //             .ExecuteDeleteAsync();
    //
    //
    //         _db.SaveChangesAsync();
    //         return true;
    //     }
    // }
    //
    // public bool SaveAnnotations(Annotation[] annotations)
    // {
    //     if (_db is null)
    //         return false;
    //
    //     lock (_locker)
    //     {
    //         if (annotations is null || !annotations.Any())
    //             return false;
    //
    //         _db.Annotations
    //             .AddRangeAsync(annotations);
    //         _db.SaveChangesAsync();
    //
    //         return true;
    //     }
    // }
    //
    // public int GetLastIndexAnnotation()
    // {
    //     if (_db is null)
    //         return -1;
    //
    //     lock (_locker)
    //     {
    //         var lastIdAnnot = _db.Annotations
    //             .AsNoTracking()
    //             .OrderBy(a => a.Id)
    //             .LastOrDefaultAsync();
    //
    //         if (lastIdAnnot is null)
    //             return -1;
    //
    //         return lastIdAnnot.Id;
    //     }
    // }
    //
    // public bool InsertImageFrames(ImageFrame[]? frames)
    // {
    //     lock (_locker)
    //     {
    //         if (frames is null || !frames.Any() || _db is null)
    //             return false;
    //
    //         _db.ImageFrames.AddRangeAsync(frames);
    //         _db.SaveChangesAsync();
    //         return true;
    //     }
    // }
    //

    //
    // public bool SaveInformationDto(InformationDto frame)
    // {
    //     lock (_locker)
    //     {
    //         if (frame is null || _db is null)
    //             return false;
    //
    //         _db.InformationState.AddAsync(frame);
    //         _db.SaveChangesAsync();
    //
    //         return true;
    //     }
    // }
    //
    // public InformationDto[] GetInformationDto()
    // {
    //     if (_db is null)
    //         return [];
    //
    //     lock (_locker)
    //     {
    //         var annotations = _db.InformationState
    //             .AsNoTracking()
    //             .ToArray();
    //
    //         return annotations;
    //     }
    // }
}