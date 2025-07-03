using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
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
    private SemaphoreSlim _semaphoreSlim = new(1, 1);
    private readonly int _timeWaitSeamaphore = 10;

    public KeyMapHandler(Helper helper, MarkupData markupData, MoveImagesHandler moveImagesHandler, IMediator mediator)
    {
        _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
        _moveImagesHandler = moveImagesHandler ?? throw new ArgumentNullException(nameof(moveImagesHandler));
        _mediator = mediator;
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
        }
    }


    /// <summary>
    ///  Отслеживания движение мыши. (тутже и перемещение изображения)
    /// </summary>
    /// <param name="args"></param>
    public void HandlerOnMouseMove(MouseEventArgs args)
    {
        switch (args)
        {
            case { AltKey: true, Buttons: 1 }:
                MovingImage(args);
                break;
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
            var res = await _mediator.Send(new AddPointsQueries() { Point = points });
            if (!res)
            {
                await _mediator.Send(new SetEditAnnotBySelectPointQueries() { Point = points });
            }
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
}