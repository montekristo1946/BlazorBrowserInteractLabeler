using System.Diagnostics;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;

namespace BlazorBrowserInteractLabeler.ARM.Extension;

public class Mappers
{
    public LabelingPanelDto[] MapToLabelingPanelDto(Annotation[]? annotations,
        ColorModel[]? settingsDataColorModel,
        Label[]? markupDataLabelsName)
    {
        if (annotations is null ||
            annotations.Any() is false ||
            settingsDataColorModel is null ||
            settingsDataColorModel.Any() is false
            || markupDataLabelsName is null ||
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

    public EventCode MapIdLabelToEventCode(int idLabel)
    {
        return idLabel switch
        {
            1 => EventCode.Label1,
            2 => EventCode.Label2,
            3 => EventCode.Label3,
            4 => EventCode.Label4,
            5 => EventCode.Label5,
            6 => EventCode.Label6,
            7 => EventCode.Label7,
            8 => EventCode.Label8,
            9 => EventCode.Label9,
            10 => EventCode.Label10,
            11 => EventCode.Label11,

            _ => EventCode.None
        };
    }

    public int MapEventCodeToIdLabel(EventCode eventCode)
    {
        return eventCode switch
        {
            EventCode.Label1 => 1,
            EventCode.Label2 => 2,
            EventCode.Label3 => 3,
            EventCode.Label4 => 4,
            EventCode.Label5 => 5,
            EventCode.Label6 => 6,
            EventCode.Label7 => 7,
            EventCode.Label8 => 8,
            EventCode.Label9 => 9,
            EventCode.Label10 => 10,
            EventCode.Label11 => 11,

            _ => 0
        };
    }
}