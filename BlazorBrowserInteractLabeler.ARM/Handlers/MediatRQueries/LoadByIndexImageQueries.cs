using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class LoadByIndexImageQueries: IRequest<bool>
{
    public int IndexImage { get; set; }
}