using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class DeleteCodeKeyQueries : IRequest<bool>
{
    public CodeKey? CodeKey { get; set; }
}