using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class SetActiveLabelQueries:IRequest<bool>
{
    public int IdLabel { get; set; }
}