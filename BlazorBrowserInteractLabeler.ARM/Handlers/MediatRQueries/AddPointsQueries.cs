using BlazorBrowserInteractLabeler.ARM.Dto;
using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class AddPointsQueries : IRequest<bool>
{
    public PointT Point { get; set; }
}