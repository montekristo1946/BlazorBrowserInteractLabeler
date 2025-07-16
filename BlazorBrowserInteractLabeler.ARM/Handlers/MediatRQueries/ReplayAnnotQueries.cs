using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class ReplayAnnotQueries: IRequest<bool>
{
    public int IdImage { get; set; }
}