using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Properties;
using BrowserInteractLabeler.Web.Component.DrawingJobBox;
using BrowserInteractLabeler.Web.Infrastructure.Buffers;
using BrowserInteractLabeler.Web.Infrastructure.Configs;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using ILogger = Serilog.ILogger;


namespace BrowserInteractLabeler.Web.Infrastructure.Handlers;

public class NavigationHandler
{
    public Action IsNewImageRendered { get; set; } = null;
    public Action IsUpdatedUi { get; set; } = null;
    public bool SetMainFocusRootPanel { get; set; } = false;
    public ImagesPanel ImagesPanelRef { get; set; }

    private const float _defaultScale = 1.0F;

    private readonly CacheModel _cacheModel;
    private readonly IRepository _repository;
    private readonly ILogger _logger = Log.ForContext<NavigationHandler>();
    private readonly Helper _helper;
    private readonly CacheAnnotation _cacheAnnotation;
    private readonly ServiceConfigs _serviceConfigs;
    private readonly MoveImagesHandler _moveImagesHandler;
    private readonly MarkupHandler _markupHandler;

    public NavigationHandler(
        CacheModel cacheModel,
        IRepository repository,
        Helper helper,
        CacheAnnotation cacheAnnotation,
        ServiceConfigs serviceConfigs,
        MoveImagesHandler moveImagesHandler,
        MarkupHandler markupHandler)
    {
        _cacheModel = cacheModel ?? throw new ArgumentNullException(nameof(cacheModel));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        _cacheAnnotation = cacheAnnotation ?? throw new ArgumentNullException(nameof(cacheAnnotation));
        _serviceConfigs = serviceConfigs ?? throw new ArgumentNullException(nameof(serviceConfigs));
        _moveImagesHandler = moveImagesHandler ?? throw new ArgumentNullException(nameof(moveImagesHandler));
        _markupHandler = markupHandler ?? throw new ArgumentNullException(nameof(markupHandler));

        cacheModel.Images = new ImageFrame()
        {
            Images = Resources.ImagesMoq,
            Id = -1
        };

        // cacheModel.SizeDrawImage = cacheModel.ImageWindowsSize;
        cacheModel.OffsetDrawImage = new PointF() { X = 0.0F, Y = 0.0F };
        _cacheModel.CurrentIdImg = 1;
        
        UpdateSvg();

    }

    public async Task GetDbName(string dbName)
    {
        var name = Path.GetFileNameWithoutExtension(dbName);
        _cacheModel.CurrentSqlDbNames = name;
    }

    public async Task LoadFirstImg()
    {
        _cacheModel.CurrentIdImg = 1;
        _cacheModel.CurrentProgress = 0;
        await CreateStartImagesState(_cacheModel.CurrentIdImg);
        _cacheModel.LabelAll = _repository.GetAllLabels();
        _cacheModel.ColorAll = _serviceConfigs.Colors;
        SetMainFocusRootPanel = true;
    }


    private async Task CreateStartImagesState(int indexImg)
    {
        try
        {
            var imageFrame = _repository.GetImagesByIndex(indexImg);
            if (!imageFrame.Images.Any())
                return;

            _cacheModel.Images = imageFrame;
            // _cacheModel.SizeDrawImage = _helper.CalculationOptimalSize(imageFrame.SizeImage, _cacheModel.ImageWindowsSize);
            // _cacheModel.OffsetDrawImage =
            //     _helper.CalculationDefaultOffsetImg(_cacheModel.SizeDrawImage, _cacheModel.ImageWindowsSize);


            _cacheModel.ScaleCurrent = _defaultScale;
            await _cacheAnnotation.LoadAnnotationsSlowStorageAsync(_cacheModel.CurrentIdImg);
            _cacheModel.NameImages = imageFrame.NameImages;
            UpdateSvg();

            // if (ImagesPanelRef is null)
            // {
            //     _logger.Warning("[CreateStartImagesState] not init ImagesPanelRef");
            //     return;
            // }

            // await ImagesPanelRef.LoadImageJsAsync();
            
            _cacheModel.AllCountImages =  _repository.GetAllIndexImages().Length;
        }
        catch (Exception e)
        {
            _logger.Error("[CreateStartImagesState] {Exception}", e);
        }
    }

    private async Task HandlerClickNextAsync(int index)
    {
        var allIndex = _repository.GetAllIndexImages();

        if (index > allIndex.Length || index < 1)
        {
            _logger.Debug("[NavigationHandler:HandlerClickNextAsync] the list is over");
            return;
        }

        SetActiveIdAnnotation(-1);
        await SaveAnnotationAsync();
        _cacheModel.CurrentIdImg = index;

        _cacheModel.CurrentProgress = _helper.CalculationCurrentProgress(index, allIndex.Length);
        await CreateStartImagesState(index);
        _cacheModel.StatePrecess = "";
        
        IsNewImageRendered?.Invoke();
    }

    public async Task ButtonGoNextClick()
    {
        var currentId = _cacheModel.CurrentIdImg + 1;
        await HandlerClickNextAsync(currentId);
    }

    public async Task ButtonGoBackClick()
    {
        var currentId = _cacheModel.CurrentIdImg - 1;
        await HandlerClickNextAsync(currentId);
    }


    public void ButtonDefaultPositionImg()
    {
        // _cacheModel.OffsetDrawImage = _helper.CalculationDefaultOffsetImg(
        //     _cacheModel.SizeDrawImage,
        //     _cacheModel.ImageWindowsSize);
        // _cacheModel.ScaleCurrent = _defaultScale;
        throw new NotImplementedException("отключил пока что");
    }

    // public void SetRootWindowsSize(SizeF sizeBrowse)
    // {
    //     _cacheModel.ImageWindowsSize = _helper.CalculationRootWindowsSize(sizeBrowse);
    //     _cacheModel.RootWindowsSize = sizeBrowse;
    // }


    private void UpdateSvg()
    {
        _cacheModel.AnnotationsOnPanel = _cacheAnnotation.GetAllAnnotationsOnImg(_cacheModel.CurrentIdImg);
    }

    public async Task SaveAnnotationAsync()
    {
         await Task.Run(async () =>
        {
            await _cacheAnnotation.SaveAnnotationsOnSqlAsync(_cacheModel.CurrentIdImg);
            UpdateSvg();
        });
    }

    public Task UndoClick()
    {
        return Task.Run(() =>
        {
            _cacheAnnotation.RemoveLastAnnotation(_cacheModel.CurrentIdImg);
            UpdateSvg();
        });
    }

    public Task RedoClick()
    {
        return Task.Run(() =>
        {
            _cacheAnnotation.RestoreLastAnnotation(_cacheModel.CurrentIdImg);
            UpdateSvg();
        });
    }

    /// <summary>
    ///     Выбрали на редактирвоания аннотацию
    /// </summary>
    /// <param name="id"></param>
    public void SetActiveIdAnnotation(int id)
    {
        var resSetActiveAnnot = _cacheAnnotation.SetActiveAnnot(id);
        if (!resSetActiveAnnot.checkRes)
            return;

        _cacheModel.StatePrecess = "Active";
        var activeLabelPattern = resSetActiveAnnot.annotation.LabelPattern;
        _markupHandler.ActiveTypeLabel = activeLabelPattern;
        HandlerSetLabelId(resSetActiveAnnot.annotation.LabelId);
        SetMainFocusRootPanel = true;
        _cacheModel.ActiveTypeLabelText = _helper.CreateTypeTextToPanel(activeLabelPattern);
        _cacheModel.ActiveTypeLabel = activeLabelPattern;
        IsNewImageRendered.Invoke();
        IsUpdatedUi.Invoke();
    }

    /// <summary>
    ///     Скрывает аннотацию
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public void SetHiddenIdAnnotation(int id)
    {
        var resSetActiveAnnot = _cacheAnnotation.SetHiddenAnnot(id);
        if (!resSetActiveAnnot.checkRes)
            return;

        _cacheModel.StatePrecess = "Hidden";
        var activeLabelPattern = resSetActiveAnnot.annotation.LabelPattern;
        _markupHandler.ActiveTypeLabel = activeLabelPattern;
        HandlerSetLabelId(resSetActiveAnnot.annotation.LabelId);
        SetMainFocusRootPanel = true;
        _cacheModel.ActiveTypeLabelText = _helper.CreateTypeTextToPanel(activeLabelPattern);
        _cacheModel.ActiveTypeLabel = activeLabelPattern;
        // UpdateSvg();
        IsUpdatedUi.Invoke();
    }

    /// <summary>
    ///     Скрыть все лейблы
    /// </summary>
    /// <param name="isHidden"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public void HiddenAllLabels(bool isHidden)
    {
        if (isHidden)
        {
            _cacheModel.StatePrecess = "Hidden";
            var resSetHiddenAllAnnot = _cacheAnnotation.SetHiddenAllAnnot();
        }
        else
        {
            _cacheModel.StatePrecess = "";
            var resSetHiddenAllAnnot = _cacheAnnotation.SetFinalizeAllAnnot();
        }
       
        IsUpdatedUi.Invoke();
    }

    public void DeleteAnnotation()
    {
        _cacheAnnotation.DeleteAnnotation();
        UpdateSvg();
        SetMainFocusRootPanel = true;
    }


    /// <summary>
    ///     keyBoard [E]
    /// </summary>
    public void EventEditAnnot()
    {
        _cacheAnnotation.EventEditAnnot(_cacheModel.CurrentIdImg);
        var resultGetEditAnnotation = _cacheAnnotation.GetEditAnnotation();
        _cacheModel.StatePrecess = resultGetEditAnnotation.checkResult ? "Create" : "";

        UpdateSvg();
    }

    /// <summary>
    ///     key q,w,a,s
    /// </summary>
    /// <param name="typeLabel"></param>
    public void EnableTypeLabel(TypeLabel typeLabel)
    {
        _markupHandler.ActiveTypeLabel = typeLabel;
        _cacheModel.ActiveTypeLabelText = _helper.CreateTypeTextToPanel(typeLabel);
        _cacheModel.ActiveTypeLabel = typeLabel;

        _cacheAnnotation.EventEditAnnotForceCreateNew(_cacheModel.CurrentIdImg, typeLabel);
        var resultGetEditAnnotation = _cacheAnnotation.GetEditAnnotation();
        _cacheModel.StatePrecess = resultGetEditAnnotation.checkResult ? "Create" : "";

        UpdateSvg();
    }

    public void HandlerSetLabelId(int activeIdLabel)
    {
        _markupHandler.ActiveIdLabel = activeIdLabel;
        _cacheAnnotation.SetActiveIdLabel(activeIdLabel);
        var color = _helper.CreateColorTextToPanel(activeIdLabel, _cacheModel.ColorAll);
        _cacheModel.ActiveLabelColor = color;

        UpdateSvg();
    }


    /// <summary>
    ///     Зум изображения
    /// </summary>
    /// <param name="args"></param>
    /// <param name="now"></param>
    public void WheelDrawingPanelMouseEventAsync(WheelEventArgs args, DateTime now)
    {
        // var (scale, offset) = _helper.CalculationScale(args,
        //     _cacheModel.ScaleCurrent,
        //     _cacheModel.SizeDrawImage,
        //     _cacheModel.OffsetDrawImage,
        //     _cacheModel.ImageWindowsSize);
        // _cacheModel.OffsetDrawImage = offset;
        // _cacheModel.ScaleCurrent = scale;
        // UpdateSvg();
    }

    /// <summary>
    ///     Перемешение изобаржения первое нажатие
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="timeClick"></param>
    public void HandlerImagesPanelOnmousedown(MouseEventArgs mouseEventArgs, DateTime timeClick)
    {
        _moveImagesHandler.HandlerOnmousedown(mouseEventArgs, timeClick);
    }

    /// <summary>
    ///     Перемешение изобаржения остановка
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="timeClick"></param>
    public void HandlerImagesPanelOnmouseUp()
    {
        _moveImagesHandler.HandlerImagesPanelOnmouseUp();
    }

    /// <summary>
    ///     Перемешение изображения
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="timeClick"></param>
    public void HandlerDrawingPanelOnmousemoveAsync(MouseEventArgs mouseEventArgs)
    {
        var stepCoeff = 1 / _cacheModel.ScaleCurrent;
        var moveDist = _moveImagesHandler.HandlerOnMouseMove(mouseEventArgs, stepCoeff);
        if (moveDist.res)
            _cacheModel.OffsetDrawImage = _helper.CorrectOffset(moveDist.moveDist, _cacheModel.OffsetDrawImage);
    }

    /// <summary>
    ///     Left button Mouse, 
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="now"></param>
    public void HandleImagePanelMouseAsync(MouseEventArgs mouseEventArgs, DateTime now)
    {
        // _logger.Debug("[HandleImagePanelMouseAsync] {@MouseEventArgs}",mouseEventArgs);

        var resultGetEditAnnotation = _cacheAnnotation.GetEditAnnotation();
        if (!resultGetEditAnnotation.checkResult)
            return;

        var sizeImg = _cacheModel.SizeDrawImage;

        var (checkResult, annotation) =
            _markupHandler.HandleMouseClickAsync(mouseEventArgs, sizeImg, resultGetEditAnnotation.annot);

        if (checkResult is false)
            return;


        _cacheAnnotation.UpdateAnnotation(annotation);
        UpdateSvg();
    }


    /// <summary>
    ///     Right button mouse
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="now"></param>
    public void HandleImagePanelMouseRightButtonAsync()
    {
        var resultGetEditAnnotation = _cacheAnnotation.GetEditAnnotation();
        if (!resultGetEditAnnotation.checkResult)
            return;

        var (checkResult, annotation) =
            _markupHandler.HandleMouseClickUndoPointAsync(resultGetEditAnnotation.annot);

        if (checkResult is false)
            return;

        _cacheAnnotation.UpdateAnnotation(annotation);
        UpdateSvg();
    }


    /// <summary>
    ///     Перемещение точки Выбор обекта, нажали клавишу
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="timeClick"></param>
    public void HandlerSelectPoint(MouseEventArgs mouseEventArgs, DateTime timeClick)
    {
        var resultGetEditAnnotation = _cacheAnnotation.GetEditAnnotation();
        if (!resultGetEditAnnotation.checkResult)
            return;

        _markupHandler.PointSelection(mouseEventArgs,
            resultGetEditAnnotation.annot,
            timeClick,
            _cacheModel.SizeDrawImage,
            _cacheModel.ScaleCurrent);
    }

    /// <summary>
    ///      Перемещение точки Выбор обекта, отпустили клавишу
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="now"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void ResetSelectPointAsync()
    {
        _markupHandler.ResetSelectPoint();
    }

    /// <summary>
    ///     Перемещение точки 
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="timeClick"></param>
    public void HandlerMovePointAsync(MouseEventArgs mouseEventArgs, DateTime timeClick)
    {
        var resultGetEditAnnotation = _cacheAnnotation.GetEditAnnotation();
        if (!resultGetEditAnnotation.checkResult)
            return;

        var resHandlerOnmouseuplAsync = _markupHandler.HandlerOnmouseuplAsync(mouseEventArgs,
            resultGetEditAnnotation.annot,
            timeClick,
            _cacheModel.SizeDrawImage);

        if (!resHandlerOnmouseuplAsync.result)
            return;

        _cacheAnnotation.UpdateAnnotation(resHandlerOnmouseuplAsync.annot);
        UpdateSvg();
    }

    public async Task ButtonEnterIdActiveIdImagesAsync(int indexImg)
    {
        // var resultTryParse = Int32.TryParse(indexImgString, out var indexImg);
        // if (!resultTryParse)
        //     return;

        await HandlerClickNextAsync(indexImg);
    }

    public void CancelFocusRootPanelAsync()
    {
        SetMainFocusRootPanel = false;
       
    }


    public PointF CalculateCursor(double argOffsetX, double argOffsetY)
    {
        var currentX = (argOffsetX / _cacheModel.SizeDrawImage.Width);
        var currentY = (argOffsetY / _cacheModel.SizeDrawImage.Height);

        return new PointF() { X = (float)currentX, Y = (float)currentY };
    }


    /// <summary>
    ///     Перестроить порядок точек в фируге
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="reverse"></param>
    /// <param name="remove"></param>
    public void HandlerRepositioningPoints(MouseEventArgs mouseEventArgs, bool reverse, bool remove)
    {
        var resultGetEditAnnotation = _cacheAnnotation.GetEditAnnotation();
        if (!resultGetEditAnnotation.checkResult)
            return;

        var (checkResult, annotation) =
            _markupHandler.RepositioningPoints(resultGetEditAnnotation.annot, reverse, remove);
        if (checkResult is false)
            return;

        _cacheAnnotation.UpdateAnnotation(annotation);
        HandlerSelectPoint(mouseEventArgs, DateTime.Now);
        UpdateSvg();
    }
}