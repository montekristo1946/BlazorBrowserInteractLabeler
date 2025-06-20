using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using Microsoft.AspNetCore.Components.Web;
namespace BlazorBrowserInteractLabeler.ARM.Handlers;

public class KeyMapHandler
{
    private readonly Helper _helper;
    private readonly MarkupData  _markupData;
    private readonly MoveImagesHandler  _moveImagesHandler;
    private const int LeftButton = 1;
    private const int RightButton = 2;
    
    public KeyMapHandler(Helper helper, MarkupData markupData, MoveImagesHandler moveImagesHandler)
    {
        _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
        _moveImagesHandler = moveImagesHandler ?? throw new ArgumentNullException(nameof(moveImagesHandler));
    }

 
    
    /// <summary>
    /// Нажатие клавиш мыши.
    /// </summary>
    /// <param name="args"></param>
    public void HandlerOnMouseDown(MouseEventArgs args)
    {
        switch (args)
        {
            case { CtrlKey: false, AltKey: false , Buttons: LeftButton }:
                CreatePoint(args);
                break;
            case { CtrlKey: false, AltKey: true , Buttons: LeftButton }:
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
        var scale = _helper.CalculationScale(args.DeltaY, _markupData.ScaleCurrent);
        _markupData.ScaleCurrent = scale;

    }


    private void StartMoveImage(MouseEventArgs args)
    {
        var correctPoint = _helper.GetAbsoluteCoordinate(
            args.PageX,
            args.PageY, 
            _markupData.ImageMarkerPanelSize);
        
        _moveImagesHandler.HandlerOnmousedown(correctPoint);
    }

    private void CreatePoint(MouseEventArgs args)
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
        
        _markupData.TestDrawPoint = points;
    }
    
    private void MovingImage(MouseEventArgs args)
    {
        var correctPoint = _helper.GetAbsoluteCoordinate(
            args.PageX,
            args.PageY, 
            _markupData.ImageMarkerPanelSize);
        
        var stepCoeff = 1 / _markupData.ScaleCurrent;
        
        var (res,offset) = _moveImagesHandler.HandlerOnMouseMove(correctPoint, stepCoeff);

        if(!res)
            return;

        _markupData.OffsetDrawImage = new PointT()
        {
            X =  _markupData.OffsetDrawImage.X+offset.X,
            Y =  _markupData.OffsetDrawImage.Y+offset.Y
        };

    }


}