using FeeLink.Api.Common.Controllers;
using FeeLink.Api.Common.Requests;
using FeeLink.Api.Common.WebSockets;
using FeeLink.Api.WebSockets;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Toys.Common;
using FeeLink.Application.UseCases.Toys.Queries.ListLinkedWithUser;
using FeeLink.Domain.Common.Errors;
using FeeLink.Infrastructure.Common.Wearable;
using FeeLink.Infrastructure.Services.WebSocket;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FeeLink.Api.Controllers;

public class ToysController(
    IMediator mediator,
    IToyRepository toyRepository,
    WebSocketConnectionManager webSocketConnectionManager,
    IAuthService authService) : ApiController
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

    [HttpPost("switch-data-sending/{macAddress}")]
    public async Task<IActionResult> SwitchDataSending(string macAddress, [FromBody] bool sendData)
    {
        var userIdOrError = authService.GetUserId();
        if (userIdOrError.IsError)
            return Problem(userIdOrError.Errors);
        
        var linkedToysResult = await mediator.Send(new ListLinkedToysWithUserQuery(userIdOrError.Value));
        
        if (linkedToysResult.IsError)
            return Problem(linkedToysResult.Errors);
        
        if (linkedToysResult.Value.Items.All(t => t.MacAddress != macAddress))
            return Problem(Errors.Toy.NotLinkedToUser);

        var ws = webSocketConnectionManager.GetSocket(macAddress);

        if (ws is null)
            return Problem(Errors.Toy.TurnedOff);

        await ws.SendCommand(new SensorDataWS.WearableCommandRequest<SwitchWearableDataSending>(
            WearableCommandOptions.SwitchSendingStatus,
            new SwitchWearableDataSending { MacAddress = macAddress, SendData = sendData }));

        return Ok();
    }
    
    [HttpGet("data-sending-status/{macAddress}")]
    public async Task<IActionResult> GetDataSendingStatus(string macAddress)
    {
        var userIdOrError = authService.GetUserId();
        if (userIdOrError.IsError)
            return Problem(userIdOrError.Errors);
        
        var linkedToysResult = await mediator.Send(new ListLinkedToysWithUserQuery(userIdOrError.Value));
        
        if (linkedToysResult.IsError)
            return Problem(linkedToysResult.Errors);
        
        if (linkedToysResult.Value.Items.All(t => t.MacAddress != macAddress))
            return Problem(Errors.Toy.NotLinkedToUser);

        var ws = webSocketConnectionManager.GetSocket(macAddress);

        if (ws is null)
            return Problem(Errors.Toy.TurnedOff);

        await ws.SendCommand(new SensorDataWS.WearableCommandRequest<Unit> (WearableCommandOptions.GetDataSendingStatus, Unit.Value));

        return Ok();
    }
}