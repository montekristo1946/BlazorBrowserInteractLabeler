using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;

namespace BlazorBrowserInteractLabeler.ARM.Extension;

public class Mappers
{
    public LabelingPanelDto[] MapToLabelingPanelDto(
        Annotation[]? annotations, 
        ColorModel[]? settingsDataColorModel,
        Label[]? markupDataLabelsName)
    {
        if (annotations is null || 
            annotations.Any() is false ||
            settingsDataColorModel is null ||
            settingsDataColorModel.Any() is false || markupDataLabelsName is null ||
            markupDataLabelsName.Any() is false)
            return [];

        var retValues = annotations.Select(p =>
        {
            var idLabel = p.LabelId;
            var color = settingsDataColorModel.FirstOrDefault(c => c.IdLabel == idLabel)?.Color ?? string.Empty;
            var name = markupDataLabelsName.FirstOrDefault(label => label.Id == idLabel)?.NameLabel ?? string.Empty;
            
            return new LabelingPanelDto()
            {
                State = p.State,
                Color = color,
                IdAnnotation = p.Id,
                LabelPattern = p.LabelPattern,
                Name = name
            };
        }).ToArray();

        return retValues;
    }
}