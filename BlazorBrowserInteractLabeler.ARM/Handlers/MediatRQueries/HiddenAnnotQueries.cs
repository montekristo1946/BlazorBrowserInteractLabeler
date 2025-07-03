using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class HiddenAnnotQueries:IRequest<bool>
{
    public int IdAnnotaion { get; set; }
}