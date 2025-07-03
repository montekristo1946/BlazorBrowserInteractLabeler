using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class EditionAnnotQueries:IRequest<bool>
{
    public int IdAnnotaion { get; set; }
}