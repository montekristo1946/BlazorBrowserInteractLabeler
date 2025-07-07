using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common.DTO;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRHandlers;

/// <summary>
/// Перемещение точки в Annot.
/// </summary>
public class MovingInitPointHandler : IRequestHandler<MovingInitPointQueries, bool>
{
    private readonly ILogger _logger = Log.ForContext<MovingInitPointHandler>();
    private readonly AnnotationHandler _annotationHandler;
    private readonly MarkupData _markupData;
    private readonly MovingPointData _movingPointData;
    private readonly IMediator _mediator;
    private const int SizeAreaPx = 10;
    
    public MovingInitPointHandler( AnnotationHandler annotationHandler, MarkupData markupData, MovingPointData movingPointData, IMediator mediator)
    {
        _annotationHandler = annotationHandler ?? throw new ArgumentNullException(nameof(annotationHandler));
        _markupData = markupData;
        _movingPointData = movingPointData ?? throw new ArgumentNullException(nameof(movingPointData));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(MovingInitPointQueries? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return false;
          
            var allAnnots = await _annotationHandler.GetAllAnnotations();
            var currentAnnot = allAnnots.FirstOrDefault(p => p.State == StateAnnot.Edit);
            if (currentAnnot is null)
                return false;

            var points = currentAnnot.Points;
            if (points is null || !points.Any())
                return false;

            
                
            var newPoint = request.Point;
            var overlapPercentageX = SizeAreaPx / _markupData.SizeConvas.Width;
            var overlapPercentageY = SizeAreaPx / _markupData.SizeConvas.Width;
            var movingPoint = points.FirstOrDefault(p =>
            {
                var deltaX = Math.Abs(p.X - newPoint.X);
                var deltaY = Math.Abs(p.Y - newPoint.Y);
                var res = deltaX <= overlapPercentageX && deltaY <= overlapPercentageY;

                return res;
            });
            
            if (movingPoint is null)
                return false;

            _movingPointData.CurrentIdAnnot = currentAnnot.Id;
            _movingPointData.PositionInGroup = movingPoint.PositionInGroup;
            
            await _mediator.Send(new MovingPointQueries() {Point = newPoint}, cancellationToken);
            return true;
        }
        catch (Exception e)
        {
            _logger.Error("[MovingPointsHandler] {@Exception}", e);
        }


        return false;
    }


}