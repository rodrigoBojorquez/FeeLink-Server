using FeeLink.Api.Common.Controllers;
using FeeLink.Api.Common.WebSockets;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Toys.Common;
using FeeLink.Application.UseCases.Toys.Queries.ListLinkedWithUser;
using FeeLink.Domain.Common.Errors;
using FeeLink.Infrastructure.Common.Wearable;
using FeeLink.Infrastructure.Services.WebSocket;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FeeLink.Api.Controllers;

public class ToysController(IMediator mediator, IToyRepository toyRepository, WebSocketConnectionManager webSocketConnectionManager) : ApiController
{
    [HttpGet("linked/{userId:guid}")]
    public async Task<IActionResult> ListLinkedWithUser(Guid userId)
    {
        var query = new ListLinkedToysWithUserQuery(userId);
        var result = await mediator.Send(query);

        return result.Match(Ok, Problem);
    }

    [HttpGet("{macAddress}")]
    public async Task<IActionResult> GetToyByMacAddress(string macAddress)
    {
        var toy = await toyRepository.GetByMacAsync(macAddress);

        return toy is null ? Problem(Errors.Toy.NotFound) : Ok(toy.ToResult());
    }
    
    // [HttpPost("stop-data-sending/{macAddress}")]
    // public async Task<IActionResult> SwitchDataSending(string macAddress)
    // {
    //     var toy = await toyRepository.GetByMacAsync(macAddress);
    //
    //     if (toy is null)
    //         return Problem(Errors.Toy.NotFound);
    //
    //     var ws = webSocketConnectionManager.GetSocket(macAddress);
    //
    //     if (ws is null)
    //         return Problem(Errors.Toy.TurnedOff);
    //
    //     await ws.SendCommand(WearableCommandOptions.Stop, new { Message = "Apagar" });
    //
    //     return Ok();
    // }
}