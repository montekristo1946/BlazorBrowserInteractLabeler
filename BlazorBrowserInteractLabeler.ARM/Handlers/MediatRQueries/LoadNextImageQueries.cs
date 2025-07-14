using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class LoadNextImageQueries : IRequest<bool>
{
    public bool IsForward { get; set; }
}