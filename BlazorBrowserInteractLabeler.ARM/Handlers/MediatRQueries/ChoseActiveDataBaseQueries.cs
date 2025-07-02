using MediatR;

namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class ChoseActiveDataBaseQueries:IRequest<bool>
{
    public string PathDb { get; set; }
}