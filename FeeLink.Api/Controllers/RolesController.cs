using FeeLink.Api.Common.Controllers;
using FeeLink.Application.UseCases.Roles.Queries;
using FeeLink.Application.UseCases.Roles.Queries.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FeeLink.Api.Controllers;

public class RolesController(IMediator mediator) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var result = await mediator.Send(new ListRolesQuery());
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRole(Guid id)
    {
        var result = await mediator.Send(new GetRoleQuery(id));
        return result.Match(Ok, Problem);
    }
}