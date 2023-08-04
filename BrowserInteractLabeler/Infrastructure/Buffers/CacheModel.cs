using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;

namespace BrowserInteractLabeler.Infrastructure;

/// <summary>
/// Это приблуда будет храниться в json формате рядом с исполняемым файлом.. необходим для возрата к последней выполняемой задаче
/// </summary>
public class CacheModel
{
    public int CurrentIdImg { get; set; } = 0;
    internal string ImagesBase64 => $"data:image/jpg;base64," + Convert.ToBase64String(Images.Images);
    internal ImageFrame Images { get; set; } = new ImageFrame();
    
    internal string CssScale =>
        $"transform: scale({ScaleCurrent}) translate({OffsetDrawImage.X}px, {OffsetDrawImage.Y}px)";

    internal float ScaleCurrent  { get; set; } = 1.0F;
    internal PointF OffsetDrawImage { get; set; } = new();
    internal SizeF SizeDrawImage { get; set; } = new();
    internal string WidthImg => $"{(int)SizeDrawImage.Width}px";
    internal string HeightImg => $"{(int)SizeDrawImage.Height}px";
    
    internal SizeF RootWindowsSize { get; set; } = new() { Width = 1600, Height = 800 };
    internal SizeF ImageWindowsSize { get; set; }= new() { Width = 1600, Height = 800 };
    internal string WidthMainWin => $"{(int)ImageWindowsSize.Width}px";
    internal string HeightMainWin => $"{(int)ImageWindowsSize.Height}px";
    internal Annotation[] AnnotationsOnPanel { get; set; } = Array.Empty<Annotation>();
    internal Label[] LabelAll { get; set; } = Array.Empty<Label>();
    internal ColorModel[] ColorAll { get; set; } = Array.Empty<ColorModel>();
    public string ActiveTypeLabelText { get; set; } = "";
    public TypeLabel ActiveTypeLabel { get; set; } = TypeLabel.None;
    public string ActiveLabelColor { get; set; } = "#f8f9fa";
    public string StatePrecess { get; set; }= "";
    public string NameImages { get; set; }="";
    public int CurrentProgress { get; set; } = 0;

    public PointF PointCursor { get; set; } = new PointF() { X = 0, Y = 0 };

    // public bool DrawCrosshair { get; set; } = false;

}