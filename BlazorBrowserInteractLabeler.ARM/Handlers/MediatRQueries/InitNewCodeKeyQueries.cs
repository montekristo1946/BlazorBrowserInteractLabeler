using BlazorBrowserInteractLabeler.ARM.ViewData;
using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class InitNewCodeKeyQueries : IRequest<bool>
{
    public CodeKey? CodeKey { get; set; }

    public string? Color { get; set; }
}