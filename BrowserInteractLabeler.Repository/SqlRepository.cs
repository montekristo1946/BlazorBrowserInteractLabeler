using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BrowserInteractLabeler.Repository;

public class SqlRepository : IRepository
{
    private ApplicationDbContext _db = null!;
    private readonly ILogger _logger = Log.ForContext<SqlRepository>();
    private readonly object _locker = new();

    public bool LoadDatabase(string pathDb)
    {
        try
        {
            lock (_locker)
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
        }
        catch (Exception e)
        {
            _logger.Debug("[LoadDatabaseAsync] {Exception}", e);
        }

        return false;
    }

    public void Dispose()
    {
        _db?.Dispose();
    }

    public int[] GetAllIndexImages()
    {
        lock (_locker)
        {
            var retArr = _db.ImageFrames.Select(p => p.Id).ToArray();
            return retArr;
        }
    }

    public ImageFrame[] GetAllImages()
    {
        lock (_locker)
        {
            if (_db is null)
                return Array.Empty<ImageFrame>();

            var retArr = _db.ImageFrames.ToArray();
            return retArr;
        }
    }

    public Annotation[] GetAllAnnotations()
    {
        lock (_locker)
        {
            if (_db is null)
                return Array.Empty<Annotation>();

            var retArr = _db.Annotations.Include(p => p.Points).ToArray();
            return retArr;
        }
    }

    public ImageFrame GetImagesByIndex(int imagesId)
    {
        lock (_locker)
        {
            if (_db is null)
                return new ImageFrame();

            var res = _db.ImageFrames.Include(p => p.SizeImage)
                .FirstOrDefault(i => i.Id == imagesId);
            return res ?? new ImageFrame();
        }
    }

    public Label[] GetAllLabels()
    {
        lock (_locker)
        {
            if (_db is null)
                return Array.Empty<Label>();

            var labels = _db.Labels.ToArray();
            return labels;
        }
    }

    public Annotation[] GetAnnotationsFromImgId(int imagesId)
    {
        lock (_locker)
        {
            if (_db is null)
                return Array.Empty<Annotation>();

            var annotations = _db.Annotations
                .Include(point => point.Points)
                .Where(p => p.ImageFrameId == imagesId).ToArray();

            return annotations;
        }
    }


    public bool DeleteAnnotations(Annotation[] removeAnnot)
    {
        lock (_locker)
        {
            if (removeAnnot is null || !removeAnnot.Any() || _db is null)
                return false;

            _db.Annotations.RemoveRange(removeAnnot);
            _db.SaveChanges();
            return true;
        }
    }

    public bool SaveAnnotations(Annotation[] annotations)
    {
        lock (_locker)
        {
            if (annotations is null || !annotations.Any() || _db is null)
                return false;

            _db.Annotations.AddRangeAsync(annotations);
            _db.SaveChangesAsync();

            return true;
        }
    }

    public int GetLastIndexAnnotation()
    {
        lock (_locker)
        {
            if (_db is null)
                return -1;

            var lastIdAnnot = _db.Annotations.OrderBy(a => a.Id).LastOrDefaultAsync();

            if (lastIdAnnot is null)
                return -1;

            return lastIdAnnot.Id;
        }
    }

    public bool InsertImageFrames(ImageFrame[]? frames)
    {
        lock (_locker)
        {
            if (frames is null || !frames.Any() || _db is null)
                return false;

            _db.ImageFrames.AddRangeAsync(frames);
            _db.SaveChangesAsync();
            return true;
        }
    }

    public bool InsertLabels(Label?[]? labels)
    {
        lock (_locker)
        {
            if (labels is null || !labels.Any() || _db is null)
                return false;

            _db.Labels.AddRangeAsync(labels!);
            _db.SaveChangesAsync();

            return true;
        }
    }

    public bool SaveInformationDto(InformationDto frame)
    {
        lock (_locker)
        {
            if (frame is null || _db is null)
                return false;
            
            _db.InformationState.AddAsync(frame);
            _db.SaveChangesAsync();

            return true;
        }
    }

    public InformationDto[] GetInformationDto()
    {

        lock (_locker)
        {
            var annotations = _db.InformationState.ToArray();

            return annotations;
        }
    }
}