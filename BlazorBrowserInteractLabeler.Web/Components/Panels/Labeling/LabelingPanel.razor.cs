using BlazorBrowserInteractLabeler.ARM.Extension;
using BlazorBrowserInteractLabeler.ARM.Handlers;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BlazorBrowserInteractLabeler.Web.Components.Panels.Labeling;

public partial class LabelingPanel : ComponentBase
{
    [Inject] private AnnotationHandler AnnotationHandler { get; set; } = null!;
    [Inject] private Mappers Mappers { get; set; } = null!;
    [Inject] private SettingsData SettingsData { get; set; } = null!;
    [Inject] private MarkupData MarkupData { get; set; } = null!;
    
    [Inject] private IMediator Mediator { get; set; } = null!;
    
    [Parameter] public  Action  IsUpdateMenu { get; set; }= null!;
    
    
    private LabelingPanelDto[] _labelingPanelDtos = [];
    private ColorModel[] _colorModels = [];
    private Label[] _labelsName = [];
    private CodeKey [] _codeKeys = [];

    protected override async Task OnInitializedAsync()
    {
        _colorModels = SettingsData.ColorModel.OrderBy(p=>p.IdLabel).ToArray();
        _labelsName = MarkupData.LabelsName;
        _codeKeys = SettingsData.CodeKey;
        await LoadAnnots();
    }
    

    private async Task LoadAnnots()
    {
        var annotations = await AnnotationHandler.GetAllAnnotations();
        _labelingPanelDtos = Mappers.MapToLabelingPanelDto(annotations,_colorModels,_labelsName);
        _labelingPanelDtos = _labelingPanelDtos.OrderByDescending(p => p.IdAnnotation).ToArray();

    }
    private async Task ClickHiddenAll(bool isHidden)
    {
        await Mediator.Send(new HiddenAllAnnotQueries(){IsHidden = isHidden});
        IsUpdateMenu?.Invoke();
        await LoadAnnots();
        StateHasChanged();
        
    }

    private int GetCountAnnots()
    {
        return _labelingPanelDtos.Length;
    }
    private string GetSvgPath(TypeLabel labelPattern)
    {
        switch (labelPattern)
        {
            case TypeLabel.None:
                return "icons/014_fail_icon.svg";
            case TypeLabel.Box:
                return "icons/006_rectangle.svg";
            case TypeLabel.Polygon:
                return "icons/007_polygon.svg";
            case TypeLabel.PolyLine:
                return "icons/008_poly_line.svg";
            case TypeLabel.Point:
                return "icons/009_points.svg";
            default:
                throw new ArgumentOutOfRangeException(nameof(labelPattern), labelPattern, null);
        }
    }
    private async Task ButtonClickObjectHiddenAsync(int idAnnotation)
    {
        await Mediator.Send(new HiddenAnnotQueries(){IdAnnotaion = idAnnotation});
        IsUpdateMenu?.Invoke();
        await LoadAnnots();
        StateHasChanged();
    }

    private async Task ButtonClickObjectAsync(int idAnnotation)
    {
        await Mediator.Send(new EditionAnnotQueries(){IdAnnotaion = idAnnotation});
        IsUpdateMenu?.Invoke();
        await LoadAnnots();
        StateHasChanged();
    }

    private string GetBackroundColor(StateAnnot state)
    {
        return state switch
        {
            StateAnnot.None => "#ebf0ff",
            StateAnnot.Edit => "#fdccd6",
            StateAnnot.Active => "#fdccd6",
            StateAnnot.Finalized => "#ebf0ff",
            StateAnnot.Hidden => "#d4d4d4",
            _ => "#ebf0ff",
        };
    }

    public void UpdateUi()
    {
        InvokeAsync(async () =>
        {
            await LoadAnnots();
            StateHasChanged();
        });
    
    }
    
    private async Task ClickDeleteEditionAnnot()
    {
        await Mediator.Send(new DeleteEditionAnnotQueries());
        IsUpdateMenu?.Invoke();
    }

    private string GetLabelText(int idLabel)
    {
        var retValue = _labelsName.FirstOrDefault(p=>p.Id == idLabel)?.NameLabel ?? string.Empty;
        return retValue;
    }

    private async Task ButtonClickSetActiveLabel(int colorModelIdLabel)
    {
        await Mediator.Send(new SetActiveLabelQueries(){IdLabel = colorModelIdLabel});
        IsUpdateMenu?.Invoke();
    }

    private string GetKeyName(int idLabel)
    {
        var eventCode = Mappers.MapIdLabelToEventCode(idLabel);
        var name = _codeKeys.FirstOrDefault(p => p.EventCode == eventCode)?.KeyFromUser ?? String.Empty;
        return name;
    }
}