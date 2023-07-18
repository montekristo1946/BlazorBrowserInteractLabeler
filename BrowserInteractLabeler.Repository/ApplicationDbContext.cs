using System.Threading.Channels;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using Microsoft.EntityFrameworkCore;

namespace BrowserInteractLabeler.Repository;

public  class ApplicationDbContext:DbContext
{
    public ApplicationDbContext()
    {
        Database.Migrate();
    }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.Migrate();
        Database.OpenConnectionAsync();
        Database.ExecuteSqlRawAsync("PRAGMA journal_mode=DELETE");
        Database.CloseConnectionAsync();
    }
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite();
        // var databaseConnectionString = $@"Data Source=/mnt/Disk_D/TMP/14.07.2023/002/001/1_2023-07-05_10-48-24-103287.db3;foreign keys=true;";
        // optionsBuilder.UseSqlite(databaseConnectionString);
    }

    public DbSet<ImageFrame> ImageFrames { get; set; }
    
    public DbSet<Annotation> Annotations { get; set; }
    
    public DbSet<PointF> Points { get; set; }
    
    public DbSet<SizeF> Sizes { get; set; }
    public DbSet<Label> Labels { get; set; }
    
}