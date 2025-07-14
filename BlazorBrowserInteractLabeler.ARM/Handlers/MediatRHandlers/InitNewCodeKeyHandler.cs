using BlazorBrowserInteractLabeler.ARM.Extension;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Добавляет горячую клавишу в конфиг.
/// </summary>
public class InitNewCodeKeyHandler : IRequestHandler<InitNewCodeKeyQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<InitNewCodeKeyHandler>();
    private readonly SettingsData _settingsData;
    private readonly Mappers _mappers;

    public InitNewCodeKeyHandler(SettingsData settingsData, Mappers mappers)
    {
        _settingsData = settingsData ?? throw new ArgumentNullException(nameof(settingsData));
        _mappers = mappers ?? throw new ArgumentNullException(nameof(mappers));
    }

    public Task<bool> Handle(InitNewCodeKeyQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null || request.CodeKey is null || request.CodeKey.EventCode == EventCode.None)
                return Task.FromResult(false);

            var idLabel = _mappers.MapEventCodeToIdLabel(request.CodeKey.EventCode);

            CheckDuplicateCodeKey(request.CodeKey);
            InitToolsCodekey(request.CodeKey);

            if (idLabel > 0)
            {
                InitLabelCodeKey(request.Color, idLabel);
            }

            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            _logger.Error("[InitNewCodeKeyHandler] {@Exception}", e);
        }

        return Task.FromResult(false);
    }

    private void CheckDuplicateCodeKey(CodeKey requestCodeKey)
    {
        var keysSrc = _settingsData.CodeKey;
        var findCodeKey = keysSrc.FirstOrDefault(p => p.CodeFromKeyBoard == requestCodeKey.CodeFromKeyBoard);
        if (findCodeKey != null)
            throw new InvalidOperationException($"Клавиша {requestCodeKey.CodeFromKeyBoard} уже существует");

    }

    private void InitToolsCodekey(CodeKey requestCodeKey)
    {
        var keysSrc = _settingsData.CodeKey;
        keysSrc = keysSrc.Append(requestCodeKey).ToArray();
        _settingsData.CodeKey = keysSrc;
    }

    private void InitLabelCodeKey(string? requestColor, int idLabel)
    {
        if (string.IsNullOrWhiteSpace(requestColor))
            throw new InvalidOperationException("Не заполнен цвет для клавиши labeling");


        var colors = _settingsData.ColorModel;

        var sortColor = colors.Where(p => p.IdLabel != idLabel).ToList();

        sortColor.Add(new ColorModel()
        {
            Color = requestColor,
            IdLabel = idLabel
        });

        _settingsData.ColorModel = sortColor.ToArray();
    }
}