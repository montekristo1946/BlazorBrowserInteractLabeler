using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Extension;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Microsoft.AspNetCore.Components.Web;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers;

public class KeyMapHandler
{
    private readonly Helper _helper;
    private readonly MarkupData _markupData;
    private readonly MoveImagesHandler _moveImagesHandler;
    private readonly IMediator _mediator;
    private const int LeftButton = 1;
    private const int RightButton = 2;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private readonly int _timeWaitSeamaphore = 1;
    private readonly Mappers _mappers;

    public Action IsNeedUpdateUi;

    private readonly SettingsData _settingsData;
    public KeyMapHandler(
        Helper helper, 
        MarkupData markupData, 
        MoveImagesHandler moveImagesHandler, 
        IMediator mediator, 
        SettingsData settingsData,
        Mappers mappers)
    {
        _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
        _moveImagesHandler = moveImagesHandler ?? throw new ArgumentNullException(nameof(moveImagesHandler));
        _mediator = mediator;
        _settingsData = settingsData ?? throw new ArgumentNullException(nameof(settingsData));
        _mappers = mappers ?? throw new ArgumentNullException(nameof(mappers));
    }


    /// <summary>
    /// Нажатие клавиш мыши.
    /// </summary>
    /// <param name="args"></param>
    public async Task HandlerOnMouseDown(MouseEventArgs args)
    {
       switch (args)
        {
            case { CtrlKey: false, AltKey: false, Buttons: LeftButton }:
                await CreatePoint(args);
                break;
            case { CtrlKey: false, AltKey: true, Buttons: LeftButton }:
                StartMoveImage(args);
                break;
            case { CtrlKey: false, AltKey: false, Buttons: RightButton }:
                await DeleteLastPoint(args);
                break;
        }
    }

    private async Task DeleteLastPoint(MouseEventArgs args)
    {
        await _semaphoreSlim.WaitAsync(_timeWaitSeamaphore);
        try
        {
            await _mediator.Send(new DeleteLastPointsQueries());
        }
        catch (Exception e)
        {
            Log.Error("[DeleteLastPoint] {@Exception}", e);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }


    /// <summary>
    ///  Отслеживания движение мыши. (тутже и перемещение изображения)
    /// </summary>
    /// <param name="args"></param>
    public async Task  HandlerOnMouseMove(MouseEventArgs args)
    {
        
        switch (args)
        {
            case { AltKey: true, Buttons: 1 }:
                MovingImage(args);
                break;
            case {AltKey: false, Buttons: 1}:
                await MovingPoint(args);
                break;
        }
    }

    private async Task MovingPoint(MouseEventArgs args)
    {
        if (!await _semaphoreSlim.WaitAsync(_timeWaitSeamaphore))
        {
            Log.Warning("[MovingPoint] the handler is busy");
            return;
        }
        
        try
        {
            var correctPoint = _helper.GetAbsoluteCoordinate(
                args.PageX,
                args.PageY,
                _markupData.ImageMarkerPanelSize);
            
            var newPoint = _helper.CorrectPoint(
                correctPoint,
                _markupData.ScaleCurrent,
                _markupData.OffsetDrawImage,
                _markupData.SizeConvas);
            
            await _mediator.Send(new MovingPointQueries() {Point = newPoint});
        }
        catch (Exception e)
        {
            Log.Error("[CreatePoint] {@Exception}", e);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
        
    }

    /// <summary>
    /// Вращение колеса мыши для зумирования изображения.
    /// </summary>
    /// <param name="args"></param>
    public void HandleMouseWheel(WheelEventArgs args)
    {
        switch (args)
        {
            case { AltKey: true, }:
                var scale = _helper.CalculationScale(args.DeltaY, _markupData.ScaleCurrent);
                _markupData.ScaleCurrent = scale;
                break;
        }
    }


    private void StartMoveImage(MouseEventArgs args)
    {
        var correctPoint = _helper.GetAbsoluteCoordinate(
            args.PageX,
            args.PageY,
            _markupData.ImageMarkerPanelSize);

        _moveImagesHandler.HandlerOnmousedown(correctPoint);
    }

    private async Task CreatePoint(MouseEventArgs args)
    {
        var correctPoint = _helper.GetAbsoluteCoordinate(
            args.PageX,
            args.PageY,
            _markupData.ImageMarkerPanelSize);

        var points = _helper.CorrectPoint(
            correctPoint,
            _markupData.ScaleCurrent,
            _markupData.OffsetDrawImage,
            _markupData.SizeConvas);


        await _semaphoreSlim.WaitAsync(_timeWaitSeamaphore);
        try
        {
            var resMovingPoint = await _mediator.Send(new MovingInitPointQueries() { Point = points });
            if (resMovingPoint)
                return;

            var resAdd = await _mediator.Send(new AddPointsQueries() { Point = points });
            if (resAdd)
                return;

            await _mediator.Send(new SetEditAnnotBySelectPointQueries() { Point = points });
        }
        catch (Exception e)
        {
            Log.Error("[CreatePoint] {@Exception}", e);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private void MovingImage(MouseEventArgs args)
    {
        var correctPoint = _helper.GetAbsoluteCoordinate(
            args.PageX,
            args.PageY,
            _markupData.ImageMarkerPanelSize);

        var stepCoeff = 1 / _markupData.ScaleCurrent;

        var (res, offset) = _moveImagesHandler.HandlerOnMouseMove(correctPoint, stepCoeff);

        if (!res)
            return;

        _markupData.OffsetDrawImage = new PointT()
        {
            X = _markupData.OffsetDrawImage.X + offset.X,
            Y = _markupData.OffsetDrawImage.Y + offset.Y
        };
    }

    public async Task HandleKeyDownAsync(KeyboardEventArgs arg)
    {
        if (arg.Repeat)
            return;
        
        var codeKey = _settingsData.CodeKey.FirstOrDefault(p=>p.CodeFromKeyBoard == arg.Code);
        if(codeKey == null)
            return;

        await SendQueries(codeKey.EventCode);

    }

    private async Task SendQueries(EventCode arg)
    {
        switch (arg)
        {
      
            case EventCode.GoNext:
                await _mediator.Send(new LoadNextImageQueries() { IsForward = true });
                break;
            case EventCode.GoBack:
                await _mediator.Send(new LoadNextImageQueries() { IsForward = false });
                break;
            
            case EventCode.SaveAnnotation:
                await _mediator.Send(new SaveAnnotationsOnSlowStorageQueries());
                break;
            
            case EventCode.UndoAction:
                break;
            case EventCode.RedoAction:
                break;
            
            case EventCode.InitAnnotationBox:
                await _mediator.Send(new InitNewAnnotQueries() { TypeLabel = TypeLabel.Box });
                break;
            case EventCode.InitAnnotationPolygon:
                await _mediator.Send(new InitNewAnnotQueries() { TypeLabel = TypeLabel.Polygon });
                break;
            case EventCode.InitAnnotationPolyline:
                await _mediator.Send(new InitNewAnnotQueries() { TypeLabel = TypeLabel.PolyLine });
                break;
            case EventCode.InitAnnotationPoint:
                await _mediator.Send(new InitNewAnnotQueries() { TypeLabel = TypeLabel.Point });
                break;
            
            case EventCode.MoveDefault:
                await _mediator.Send(new RestorePositionImageQueries() );
                break;
            case EventCode.DeleteActiveAnnot:
                await _mediator.Send(new DeleteEditionAnnotQueries());
                break;
            
            case EventCode.Label1:
                await SendQueriesActiveLabel(EventCode.Label1);
                break;
            case EventCode.Label2:
                await SendQueriesActiveLabel(EventCode.Label2);
                break;
            case EventCode.Label3:
                await SendQueriesActiveLabel(EventCode.Label3);
                break;
            case EventCode.Label4:
                await SendQueriesActiveLabel(EventCode.Label4);
                break;
            case EventCode.Label5:
                await SendQueriesActiveLabel(EventCode.Label5);
                break;
            case EventCode.Label6:
                await SendQueriesActiveLabel(EventCode.Label6);
                break;
            case EventCode.Label7:
                await SendQueriesActiveLabel(EventCode.Label7);
                break;
            case EventCode.Label8:
                await SendQueriesActiveLabel(EventCode.Label8);
                break;
            case EventCode.Label9:
                await SendQueriesActiveLabel(EventCode.Label9);
                break;
            case EventCode.Label10:
                await SendQueriesActiveLabel(EventCode.Label10);
                break;
            case EventCode.Label11:
                await SendQueriesActiveLabel(EventCode.Label11);
                break;
                
        }
        
        IsNeedUpdateUi?.Invoke();
    }

    private async Task SendQueriesActiveLabel(EventCode label)
    {
        var id = _mappers.MapEventCodeToIdLabel(label);
        await _mediator.Send(new SetActiveLabelQueries(){IdLabel = id});
    }
}