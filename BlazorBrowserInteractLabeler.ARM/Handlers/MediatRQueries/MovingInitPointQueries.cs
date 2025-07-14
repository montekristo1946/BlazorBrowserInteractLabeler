using BlazorBrowserInteractLabeler.ARM.Dto;
using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class MovingInitPointQueries : IRequest<bool>
{
    public PointT Point { get; set; }

}