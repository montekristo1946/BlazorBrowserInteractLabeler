using BrowserInteractLabeler.Common.DTO;
using MediatR;
namespace BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;

public class InitNewAnnotQueries : IRequest<bool>
{
    public TypeLabel TypeLabel { get; set; }
}