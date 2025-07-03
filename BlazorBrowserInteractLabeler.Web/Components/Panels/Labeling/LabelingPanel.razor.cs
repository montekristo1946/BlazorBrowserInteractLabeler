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
    [Inject] private AnnotationHandler _annotationHandler { get; set; } = null!;
    [Inject] private Mappers _mappers { get; set; } = null!;
    [Inject] private SettingsData _settingsData { get; set; } = null!;
    [Inject] private MarkupData _markupData { get; set; } = null!;
    
    [Inject] private IMediator _mediator { get; set; } = null!;
    
    [Parameter] public  Action  IsUpdateMenu { get; set; }
    
    
    private bool _isHiddenState = false;
    private LabelingPanelDto[] _labelingPanelDtos = [];
    private ColorModel[] _colorModels = [];
    private Label[] _labelsName = [];

    protected override async Task OnInitializedAsync()
    {
        _colorModels = _settingsData.ColorModel;
        _labelsName = _markupData.LabelsName;
        await LoadAnnots();
    }
    

    private async Task LoadAnnots()
    {
        var annotations = await _annotationHandler.GetAllAnnotations();
        _labelingPanelDtos = _mappers.MapToLabelingPanelDto(annotations,_colorModels,_labelsName);
        _labelingPanelDtos = _labelingPanelDtos.OrderBy(p => p.IdAnnotation).ToArray();

    }
    private async Task ClickHiddenAll(bool isHidden)
    {
        await _mediator.Send(new HiddenAllAnnotQueries(){IsHidden = isHidden});
        IsUpdateMenu?.Invoke();
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
        await _mediator.Send(new HiddenAnnotQueries(){IdAnnotaion = idAnnotation});
        IsUpdateMenu?.Invoke();
        
    }

    private async Task ButtonClickObjectAsync(int idAnnotation)
    {
        await _mediator.Send(new EditionAnnotQueries(){IdAnnotaion = idAnnotation});
        IsUpdateMenu?.Invoke();
    }

    private string GetBackroundColor(StateAnnot state)
    {
        return state switch
        {
            StateAnnot.None => "#ebf0ff",
            StateAnnot.Edit => "#fdfeff",
            StateAnnot.Active => "#fdfeff",
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
        await _mediator.Send(new DeleteEditionAnnotQueries());
        IsUpdateMenu?.Invoke();
    }

    private string GetLabelText(int idLabel)
    {
        var retValue = _labelsName.FirstOrDefault(p=>p.Id == idLabel)?.NameLabel ?? string.Empty;
        return retValue;
    }

    private async Task ButtonClickSetActiveLabel(int colorModelIdLabel)
    {
        await _mediator.Send(new SetActiveLabelQueries(){IdLabel = colorModelIdLabel});
        IsUpdateMenu?.Invoke();
    }
}