using BlazorBrowserInteractLabeler.ARM.Dto;
using BlazorBrowserInteractLabeler.ARM.Handlers.MediatRQueries;
using BlazorBrowserInteractLabeler.ARM.ViewData;
using BrowserInteractLabeler.Common;
using MediatR;
using Serilog;

namespace BlazorBrowserInteractLabeler.ARM.Handlers;

public class ProjectsLocalHandler
{
    private readonly IRepository _repository;
    private readonly MarkupData _markupData;
    private readonly IMediator _mediator;

    public ProjectsLocalHandler(IRepository repository, MarkupData markupData, IMediator mediator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _markupData = markupData ?? throw new ArgumentNullException(nameof(markupData));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
  
}