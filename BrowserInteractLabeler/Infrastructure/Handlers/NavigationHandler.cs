using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Common.DTO;
using BrowserInteractLabeler.Infrastructure.Configs;
using BrowserInteractLabeler.Infrastructure.Constructors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;
using ILogger = Serilog.ILogger;


namespace BrowserInteractLabeler.Infrastructure;

public class NavigationHandler
{
    private const float _defaultScale = 1.0F;

    private readonly CacheModel _cacheModel;
    private readonly IRepository _repository;
    private readonly ILogger _logger = Log.ForContext<NavigationHandler>();
    private readonly Helper _helper;
    private readonly CacheAnnotation _cacheAnnotation;
    private readonly SvgConstructor _svgConstructor;
    private readonly ServiceConfigs _serviceConfigs;
    private readonly MoveImagesHandler _moveImagesHandler;
    private readonly MarkupHandler _markupHandler;
    private const string _cursorEnable = "url('icons/015_crosshaiir.svg') 64 64, default";

    public bool SetMainFocusRootPanel { get; set; } = false;

    public NavigationHandler(
        CacheModel cacheModel,
        IRepository repository,
        Helper helper,
        CacheAnnotation cacheAnnotation,
        SvgConstructor svgConstructor,
        ServiceConfigs serviceConfigs,
        MoveImagesHandler moveImagesHandler,
        MarkupHandler markupHandler)
    {
        _cacheModel = cacheModel ?? throw new ArgumentNullException(nameof(cacheModel));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        _cacheAnnotation = cacheAnnotation ?? throw new ArgumentNullException(nameof(cacheAnnotation));
        _svgConstructor = svgConstructor ?? throw new ArgumentNullException(nameof(svgConstructor));
        _serviceConfigs = serviceConfigs ?? throw new ArgumentNullException(nameof(serviceConfigs));
        _moveImagesHandler = moveImagesHandler ?? throw new ArgumentNullException(nameof(moveImagesHandler));
        _markupHandler = markupHandler ?? throw new ArgumentNullException(nameof(markupHandler));

        cacheModel.Images = new ImageFrame()
        {
            Images = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource/error_1.png")),
            Id = -1
        };

        cacheModel.SizeDrawImage = cacheModel.ImageWindowsSize;
        cacheModel.OffsetDrawImage = new PointF() { X = 0.0F, Y = 0.0F };
        _cacheModel.CurrentIdImg = 1;
        UpdateSvg();
    }


    public async Task LoadFirstImg()
    {
        _cacheModel.CurrentIdImg = 1;
        _cacheModel.CurrentProgress = 0;
        await CreateStartImagesState(_cacheModel.CurrentIdImg);
        _cacheModel.LabelAll = await _repository.GetAllLabelsAsync();
        _cacheModel.ColorAll = _serviceConfigs.Colors;
        SetMainFocusRootPanel = true;
    }


    private async Task CreateStartImagesState(int indexImg)
    {
        var imageFrame = await _repository.GetImagesByIndexAsync(indexImg);
        if (!imageFrame.Images.Any())
            return;

        _cacheModel.Images = imageFrame;
        _cacheModel.SizeDrawImage = _helper.CalculationOptimalSize(imageFrame.SizeImage, _cacheModel.ImageWindowsSize);
        _cacheModel.OffsetDrawImage =
            _helper.CalculationDefaultOffsetImg(_cacheModel.SizeDrawImage, _cacheModel.ImageWindowsSize);


        _cacheModel.ScaleCurrent = _defaultScale;
        await _cacheAnnotation.LoadAnnotationsSlowStorageAsync(_cacheModel.CurrentIdImg);
        _cacheModel.AnnotationsOnPanel = _cacheAnnotation.GetAllAnnotations(_cacheModel.CurrentIdImg);
        _cacheModel.NameImages = imageFrame.NameImages;
        UpdateSvg();
    }

    private async Task HandlerClickNextAsync(int index)
    {
        var allIndex = await _repository.GetAllIndexImagesAsync();

        if (index > allIndex.Length || index < 1)
        {
            _logger.Debug("[NavigationHandler:HandlerClickNextAsync] the list is over");
            return;
        }

        SetActiveIdAnnotation(-1);
        await SaveAnnotation();
        _cacheModel.CurrentIdImg = index;

        _cacheModel.CurrentProgress = _helper.CalculationCurrentProgress(index, allIndex.Length);
        await CreateStartImagesState(index);
        _cacheModel.StatePrecess = "";
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


    public async Task ButtonDefaultPositionImg()
    {
        _cacheModel.OffsetDrawImage = _helper.CalculationDefaultOffsetImg(
            _cacheModel.SizeDrawImage,
            _cacheModel.ImageWindowsSize);
        _cacheModel.ScaleCurrent = _defaultScale;
    }

    public async Task SetRootWindowsSize(SizeF sizeBrowse)
    {
        _cacheModel.ImageWindowsSize = _helper.CalculationRootWindowsSize(sizeBrowse);
    }


    private void UpdateSvg()
    {
        _cacheModel.AnnotationsOnPanel = _cacheAnnotation.GetAllAnnotations(_cacheModel.CurrentIdImg);
    }

    public Task SaveAnnotation()
    {
        return Task.Run(() =>
        {
            _cacheAnnotation.SaveAnnotationsOnSqlAsync(_cacheModel.CurrentIdImg).Wait();
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

    public  Task RedoClick()
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
        HandlerSetLabelIdAsync(resSetActiveAnnot.annotation.LabelId);
        SetMainFocusRootPanel = true;
        _cacheModel.ActiveTypeLabelText = _helper.CreateTypeTextToPanel(activeLabelPattern);
        _cacheModel.ActiveTypeLabel = activeLabelPattern;
    }

    /// <summary>
    ///     Скрывает аннотацию
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task SetHiddenIdAnnotation(int id)
    {
        var resSetActiveAnnot = _cacheAnnotation.SetHiddenAnnot(id);
        if (!resSetActiveAnnot.checkRes)
            return;

        _cacheModel.StatePrecess = "Hidden";
        var activeLabelPattern = resSetActiveAnnot.annotation.LabelPattern;
        _markupHandler.ActiveTypeLabel = activeLabelPattern;
        HandlerSetLabelIdAsync(resSetActiveAnnot.annotation.LabelId);
        SetMainFocusRootPanel = true;
        _cacheModel.ActiveTypeLabelText = _helper.CreateTypeTextToPanel(activeLabelPattern);
        _cacheModel.ActiveTypeLabel = activeLabelPattern;
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
        var textToPanel = _helper.CreateTypeTextToPanel(typeLabel);
        _cacheModel.ActiveTypeLabelText = textToPanel;
        _cacheModel.ActiveTypeLabel = typeLabel;

        _cacheAnnotation.EventEditAnnotForceCreateNew(_cacheModel.CurrentIdImg, typeLabel);
        var resultGetEditAnnotation = _cacheAnnotation.GetEditAnnotation();
        _cacheModel.StatePrecess = resultGetEditAnnotation.checkResult ? "Create" : "";


        UpdateSvg();
    }

    public void HandlerSetLabelIdAsync(int activeIdLabel)
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
        var (scale, offset) = _helper.CalculationScale(args,
            _cacheModel.ScaleCurrent,
            _cacheModel.SizeDrawImage,
            _cacheModel.OffsetDrawImage,
            _cacheModel.ImageWindowsSize);
        _cacheModel.OffsetDrawImage = offset;
        _cacheModel.ScaleCurrent = scale;
        UpdateSvg();
    }

    /// <summary>
    ///     Перемешение изобаржения первое нажатие
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    /// <param name="timeClick"></param>
    public void HandlerImagesPanelOnmousedownAsync(MouseEventArgs mouseEventArgs, DateTime timeClick)
    {
        _moveImagesHandler.HandlerOnmousedownAsync(mouseEventArgs, timeClick);
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
    public void HandlerSelectPointAsync(MouseEventArgs mouseEventArgs, DateTime timeClick)
    {
        var resultGetEditAnnotation = _cacheAnnotation.GetEditAnnotation();
        if (!resultGetEditAnnotation.checkResult)
            return;

        _markupHandler.HandlerOnmousedownAsync(mouseEventArgs,
            resultGetEditAnnotation.annot,
            timeClick,
            _cacheModel.SizeDrawImage);
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

    public async Task ButtonEnterIdActiveIdImagesAsync(string indexImgString)
    {
        // _logger.Debug($"[ButtonEnterIdActiveIdImagesAsync] {indexImgString}");
        var resultTryParse = Int32.TryParse(indexImgString, out var indexImg);
        if (!resultTryParse)
            return;

        await HandlerClickNextAsync(indexImg);
    }

    public Task CancelFocusRootPanelAsync()
    {
        SetMainFocusRootPanel = false;
        return Task.CompletedTask;
    }


    public PointF CalculateCursor(double argOffsetX, double argOffsetY)
    {
        var currentX = (argOffsetX / _cacheModel.SizeDrawImage.Width);
        var currentY = (argOffsetY / _cacheModel.SizeDrawImage.Height);

        return new PointF() { X = (float)currentX, Y = (float)currentY };
    }
}