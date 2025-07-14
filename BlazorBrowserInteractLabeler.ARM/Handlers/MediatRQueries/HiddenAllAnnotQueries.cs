using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class HiddenAllAnnotQueries : IRequest<bool>
{
    public bool IsHidden { get; set; }
}