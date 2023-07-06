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

        cacheModel.Images = new ImageFrame() { Images = File.ReadAllBytes("Resource/error_1.png"), Id = -1 };

        cacheModel.SizeDrawImage = cacheModel.ImageWindowsSize;
        cacheModel.OffsetDrawImage = new PointF() { X = 0.0F, Y = 0.0F };
        _cacheModel.CurrentIdImg = 1;
        UpdateSvg().Wait();
    }


    public async Task LoadFirstImg()
    {
        _cacheModel.CurrentIdImg = 1;
        _cacheModel.CurrentProgress = 0;
        await CreateStartImagesState(_cacheModel.CurrentIdImg);
        await _cacheAnnotation.LoadAnnotationsSlowStorage(_cacheModel.CurrentIdImg);
        _cacheModel.AnnotationsOnPanel = await _cacheAnnotation.GetAllAnnotations(_cacheModel.CurrentIdImg);
        _cacheModel.LabelAll = await _repository.GetAllLabelsAsync();
        _cacheModel.ColorAll = _serviceConfigs.Colors;
        SetMainFocusRootPanel = true;
    }
    

    private async Task CreateStartImagesState(int indexImg)
    {
        var imageFrame = await _repository.GetImagesByIndexAsync(indexImg);
        if(!imageFrame.Images.Any())
            return;
            
        _cacheModel.Images = imageFrame;
        _cacheModel.SizeDrawImage = _helper.CalculationOptimalSize(imageFrame.SizeImage, _cacheModel.ImageWindowsSize);
        _cacheModel.OffsetDrawImage =
            _helper.CalculationDefaultOffsetImg(_cacheModel.SizeDrawImage, _cacheModel.ImageWindowsSize);
        
        
        _cacheModel.ScaleCurrent = _defaultScale;
        await _cacheAnnotation.LoadAnnotationsSlowStorage(_cacheModel.CurrentIdImg);
        _cacheModel.AnnotationsOnPanel = await _cacheAnnotation.GetAllAnnotations(_cacheModel.CurrentIdImg);
        _cacheModel.NameImages = imageFrame.NameImages;
        await UpdateSvg();
    }

    private async Task HandlerClickNextAsync(int index)
    {
        var allIndex = await _repository.GetAllIndexImagesAsync();

        if (index > allIndex.Length || index<1 )
        {
            _logger.Debug("[NavigationHandler:HandlerClickNextAsync] the list is over");
            return ;
        }
        await SetActiveIdAnnotation(-1);
        await SaveAnnotation();
        _cacheModel.CurrentIdImg = index;
       
        _cacheModel.CurrentProgress = _helper.CalculationCurrentProgress(index,allIndex.Length );
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


    public async Task UpdateSvg()
    {
        _cacheModel.AnnotationsOnPanel = await _cacheAnnotation.GetAllAnnotations(_cacheModel.CurrentIdImg);
        _cacheModel.SvgModelString = await _svgConstructor.CreateSVG(_cacheModel.AnnotationsOnPanel);
    }

    public async Task SaveAnnotation()
    {
        await _cacheAnnotation.SaveAnnotationsOnSql(_cacheModel.CurrentIdImg);
    }

    public async Task UndoClick()
    {
        await _cacheAnnotation.RemoveLastAnnotation(_cacheModel.CurrentIdImg);
        await UpdateSvg();
    }

    public async Task RedoClick()
    {
        await _cacheAnnotation.RestoreLastAnnotation(_cacheModel.CurrentIdImg);
        await UpdateSvg();
    }

    /// <summary>
    ///     Изменения цвета аннотации
    /// </summary>
    /// <param name="id"></param>
    public async Task SetActiveIdAnnotation(int id)
    {
        var res = await _cacheAnnotation.SetActiveAnnot(id);
        if (!res)
            return;

        _cacheModel.StatePrecess = "Edit";
        await UpdateSvg();
    }


    public async Task DeleteAnnotation()
    {
        await _cacheAnnotation.DeleteAnnotation();
        await UpdateSvg();
        SetMainFocusRootPanel = true;
    }


    public async Task<SizeF> GetSizeDrawImage()
    {
        return _cacheModel.SizeDrawImage;
    }

    /// <summary>
    ///     keyBoard [N]
    /// </summary>
    public async Task EventEditAnnot()
    {
        await _cacheAnnotation.EventEditAnnot(_cacheModel.CurrentIdImg);
        var resultGetEditAnnotation = await _cacheAnnotation.GetEditAnnotation();
        _cacheModel.StatePrecess = resultGetEditAnnotation.result ? "Create" : "";

        await UpdateSvg();
    }

    public async Task EnableTypeLabel(TypeLabel typeLabel)
    {
        _markupHandler.ActiveTypeLabel = typeLabel;
        var textToPanel = await _helper.CreateTypeTextToPanel(typeLabel);
        _cacheModel.ActiveTypeLabel = textToPanel;
    }

    public async Task HandlerSetLabelIdAsync(int id)
    {
        _markupHandler.ActiveIdLabel = id;
        await _cacheAnnotation.SetActiveIdLabel(id);
        var color = _helper.CreateColorTextToPanel(id, _cacheModel.ColorAll);
        _cacheModel.ActiveIdLabelColor = color;

        await UpdateSvg();
    }


    /// <summary>
    ///     Зум изображения
    /// </summary>
    /// <param name="args"></param>
    /// <param name="now"></param>
    public async Task WheelDrawingPanelMouseEventAsync(WheelEventArgs args, DateTime now)
    {
        var (scale, offset) = _helper.CalculationScale(args,
            _cacheModel.ScaleCurrent,
            _cacheModel.SizeDrawImage,
            _cacheModel.OffsetDrawImage,
            _cacheModel.ImageWindowsSize);
        _cacheModel.OffsetDrawImage = offset;
        _cacheModel.ScaleCurrent = scale;
    }

    public async Task HandlerImagesPanelOnmousedownAsync(MouseEventArgs mouseEventArgs, DateTime timeClick)
    {
        await _moveImagesHandler.HandlerOnmousedownAsync(mouseEventArgs, timeClick);
    }

    public async Task HandlerDrawingPanelOnmousemoveAsync(MouseEventArgs mouseEventArgs, DateTime timeClick)
    {
        var moveDist = await _moveImagesHandler.HandlerOnmouseupAsync(mouseEventArgs, timeClick);
        if (moveDist.res)
            _cacheModel.OffsetDrawImage = _helper.CorrectOffset(moveDist.moveDist, _cacheModel.OffsetDrawImage);
    }

    public async Task HandleImagePanelMouseAsync(MouseEventArgs mouseEventArgs, DateTime now)
    {
        var sizeImg = await GetSizeDrawImage();
        var resultGetEditAnnotation = await _cacheAnnotation.GetEditAnnotation();
        if (!resultGetEditAnnotation.result)
            return;

        var (res, annotation) =
            await _markupHandler.HandleMouseClickAsync(mouseEventArgs, sizeImg, resultGetEditAnnotation.annot);
        if (res is false)
            return;
        await _cacheAnnotation.UpdateAnnotation(annotation);
        await UpdateSvg();
    }

    public async Task HandlerSelectPointAsync(MouseEventArgs mouseEventArgs, DateTime timeClick)
    {
        var resultGetEditAnnotation = await _cacheAnnotation.GetEditAnnotation();
        if (!resultGetEditAnnotation.result)
            return;

        await _markupHandler.HandlerOnmousedownAsync(mouseEventArgs,
            resultGetEditAnnotation.annot,
            timeClick,
            _cacheModel.SizeDrawImage);
    }

    public async Task HandlerMovePointAsync(MouseEventArgs mouseEventArgs, DateTime timeClick)
    {
        var resultGetEditAnnotation = await _cacheAnnotation.GetEditAnnotation();
        if (!resultGetEditAnnotation.result)
            return;

        var resHandlerOnmouseuplAsync = await _markupHandler.HandlerOnmouseuplAsync(mouseEventArgs,
            resultGetEditAnnotation.annot,
            timeClick,
            _cacheModel.SizeDrawImage);

        if (!resHandlerOnmouseuplAsync.result)
            return;

        await _cacheAnnotation.UpdateAnnotation(resHandlerOnmouseuplAsync.annot);
        await UpdateSvg();
    }

    public async Task ButtonEnterIdActiveIdImagesAsync(string indexImgString)
    {
        _logger.Debug($"[ButtonEnterIdActiveIdImagesAsync] {indexImgString}");
        var resultTryParse = Int32.TryParse(indexImgString, out var indexImg);
        if(!resultTryParse)
            return;
      
        await HandlerClickNextAsync(indexImg);
        


        
    }

    public Task CancelFocusRootPanelAsync()
    {
        SetMainFocusRootPanel = false;
        return Task.CompletedTask;
    }
}