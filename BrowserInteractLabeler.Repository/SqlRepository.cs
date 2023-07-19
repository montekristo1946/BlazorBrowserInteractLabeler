using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BrowserInteractLabeler.Repository;

public class SqlRepository : IRepository
{
    private ApplicationDbContext _db = null;
    private readonly ILogger _logger = Log.ForContext<SqlRepository>();

    public async Task<bool> LoadDatabaseAsync(string pathDb)
    {
        return await Task.Run(() =>
        {
            try
            {
                if (_db is not null)
                {
                    _db.Dispose();
                    _db = null;
                }
                _logger.Debug("[LoadDatabaseAsync] Init  {PathDb}", pathDb);
                var databaseConnectionString = $@"Data Source={pathDb};foreign keys=true;";
                var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(databaseConnectionString)
                    .Options;
                _db = new ApplicationDbContext(contextOptions);
                _logger.Debug("[LoadDatabaseAsync] Load ok {PathDb}", pathDb);
                
                return true;
            }
            catch (Exception e)
            {
                _logger.Debug("[LoadDatabaseAsync] {Exception}", e);
            }
            return false;
        });
    }

    public void Dispose()
    {
        _db?.Dispose();
    }

    public async Task<int[]> GetAllIndexImagesAsync()
    {
        if (_db is null)
            return Array.Empty<int>();

        var retArr = await _db.ImageFrames.Select(p => p.Id).ToArrayAsync();
        return retArr;
    }

    public async Task<ImageFrame[]> GetAllImagesAsync()
    {
        if (_db is null)
            return Array.Empty<ImageFrame>();
        var retArr = await _db.ImageFrames.ToArrayAsync();
        return retArr;
    }

    public async Task<Annotation[]> GetAllAnnotationsAsync()
    {
        if (_db is null)
            return Array.Empty<Annotation>();
        
        var retArr = await _db.Annotations.Include(p=>p.Points).ToArrayAsync();
        return retArr;
    }

    public async Task<ImageFrame> GetImagesByIndexAsync(int imagesId)
    {
        if (_db is null)
            return new ImageFrame();

        var res = await _db.ImageFrames.Include(p => p.SizeImage)
            .FirstOrDefaultAsync(i => i.Id == imagesId);
        if (res is null)
            return new ImageFrame();

        return res;
    }

    public async Task<Label[]> GetAllLabelsAsync()
    {
        if (_db is null)
            return Array.Empty<Label>();

        var labels = await _db.Labels.ToArrayAsync();
        return labels;
    }

    public async Task<Annotation[]> GetAnnotationsFromImgIdAsync(int imagesId)
    {
        if (_db is null)
            return Array.Empty<Annotation>();

        var annotations = await _db.Annotations
            .Include(point => point.Points)
            .Where(p => p.ImageFrameId == imagesId).ToArrayAsync();

        return annotations;
    }


    public async Task<bool> DeleteAnnotationsAsync(Annotation[] removeAnnot)
    {
        if (removeAnnot is null || !removeAnnot.Any() || _db is null)
            return false;

        _db.Annotations.RemoveRange(removeAnnot);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SaveAnnotationsAsync(Annotation[]? annotations)
    {
        if (annotations is null || !annotations.Any() || _db is null)
            return false;


        await _db.Annotations.AddRangeAsync(annotations);
        await _db.SaveChangesAsync();


        return true;
    }

    public async Task<int> GetLastIndexAnnotation()
    {
        if (_db is null)
            return -1;

        var lastIdAnnot = await _db.Annotations.OrderBy(a => a.Id).LastOrDefaultAsync();
        if (lastIdAnnot is null)
            return -1;

        return lastIdAnnot.Id;
    }

    public async Task<bool> InsertImageFrames(ImageFrame[]? frames)
    {
        if (frames is null || !frames.Any() || _db is null)
            return false;

        await _db.ImageFrames.AddRangeAsync(frames);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> InsertLabels(Label?[]? labels)
    {
        if (labels is null || !labels.Any() || _db is null)
            return false;

        await _db.Labels.AddRangeAsync(labels!);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SaveInformationDtoAsync(InformationDto frame)
    {
        if (frame is null || _db is null)
            return false;

        await _db.InformationState.AddAsync(frame);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<InformationDto[]> GetInformationDtoAsync()
    {
        if (_db is null)
            return Array.Empty<InformationDto>();

        var annotations = await _db.InformationState.ToArrayAsync();

        return annotations;
    }
}