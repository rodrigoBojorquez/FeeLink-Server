using FeeLink.Api.Common.Controllers;
using FeeLink.Application.Interfaces.Repositories;
using MediatR;

namespace FeeLink.Api.Controllers;

public class ToysController(IMediator mediator, IToyRepository toyRepository) : ApiController
{
    
}