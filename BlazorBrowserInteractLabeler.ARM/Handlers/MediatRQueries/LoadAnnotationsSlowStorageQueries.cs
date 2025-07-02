using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class LoadAnnotationsSlowStorageQueries: IRequest<bool>
{
    public int ImageId { get; set; }
}