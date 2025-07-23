using FeeLink.Api.Common.Controllers;
using FeeLink.Api.Common.Requests;
using FeeLink.Api.Common.WebSockets;
using FeeLink.Api.WebSockets;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Readings.Queries.List;
using FeeLink.Application.UseCases.Readings.Queries.SummaryReport;
using FeeLink.Application.UseCases.Toys.Commands.Create;
using FeeLink.Application.UseCases.Toys.Commands.Update;
using FeeLink.Application.UseCases.Toys.Common;
using FeeLink.Application.UseCases.Toys.Queries.ListLinkedWithUser;
using FeeLink.Domain.Common.Errors;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Common.Wearable;
using FeeLink.Infrastructure.Services.WebSocket;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FeeLink.Api.Controllers;

public class ToysController(
    IMediator mediator,
    IToyRepository toyRepository,
    ISensorReadingRepository sensorReadingRepository,
    WebSocketConnectionManager webSocketConnectionManager,
    IAuthService authService) : ApiController
{
    public record CreateToyRequest(
        string Name,
        string MacAddress,
        Guid PatientId);

    public record UpdateToyRequest(
        string Name, string MacAddress);

    public record ListSensorReadingsRequest(
        DateTime? From = null,
        DateTime? To = null,
        Metric? Metric = null,
        float? MinValue = null,
        float? MaxValue = null,
        int Page = 1,
        int PageSize = 50,
        string? SortBy = null,
        bool Desc = true
    );

    public record ListReadingsSummaryReportRequest(
        DateOnly From,
        DateOnly To,
        bool? Dummy = false);

    public record ListToysRequest(
        int? Page,
        int? PageSize,
        Guid? UserId);

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListToysRequest request)
    {
        var query = new ListToysQuery(request.Page ?? 1, request.PageSize ?? 10, request.UserId);
        var result = await mediator.Send(query);
        return result.Match(Ok, Problem);
    }

    [HttpGet("{macAddress}")]
    public async Task<IActionResult> GetToyByMacAddress(string macAddress)
    {
        var toy = await toyRepository.GetByMacAsync(macAddress);
        return toy is null ? Problem(Errors.Toy.NotFound) : Ok(toy.ToResult());
    }

    [HttpPost("{macAddress}/commands/sending-status")]
    public async Task<IActionResult> SwitchDataSending(string macAddress, [FromBody] bool sendData)
    {
        var userIdOrError = authService.GetUserId();
        if (userIdOrError.IsError)
            return Problem(userIdOrError.Errors);

        var linkedToysResult =
            await mediator.Send(new ListToysQuery(Page: 1, PageSize: 10, UserId: userIdOrError.Value));

        if (linkedToysResult.IsError)
            return Problem(linkedToysResult.Errors);

        if (linkedToysResult.Value.Items.All(t => t.MacAddress != macAddress))
            return Problem(Errors.Toy.NotLinkedToUser);

        var ws = webSocketConnectionManager.GetSocket(macAddress);

        if (ws is null)
            return Problem(Errors.Toy.TurnedOff);

        await ws.SendCommand(new SensorDataWs.WearableCommandRequest<SwitchWearableDataSending>(
            WearableCommandOptions.SwitchSendingStatus,
            new SwitchWearableDataSending { MacAddress = macAddress, SendData = sendData }));

        return Ok();
    }

    [HttpGet("{macAddress}/sending-status")]
    public async Task<IActionResult> GetDataSendingStatus(string macAddress)
    {
        var userIdOrError = authService.GetUserId();
        if (userIdOrError.IsError)
            return Problem(userIdOrError.Errors);

        var linkedToysResult =
            await mediator.Send(new ListToysQuery(Page: 1, PageSize: 10, UserId: userIdOrError.Value));

        if (linkedToysResult.IsError)
            return Problem(linkedToysResult.Errors);

        if (linkedToysResult.Value.Items.All(t => t.MacAddress != macAddress))
            return Problem(Errors.Toy.NotLinkedToUser);

        var ws = webSocketConnectionManager.GetSocket(macAddress);

        if (ws is null)
            return Problem(Errors.Toy.TurnedOff);

        await ws.SendCommand(
            new SensorDataWs.WearableCommandRequest<Unit>(WearableCommandOptions.GetDataSendingStatus, Unit.Value));

        return Ok();
    }
    
    [HttpPost("{macAddress}/commands/disconnect-wifi")]
    public async Task<IActionResult> DisconnectWifi(string macAddress)
    {
        var userIdOrError = authService.GetUserId();
        if (userIdOrError.IsError)
            return Problem(userIdOrError.Errors);

        var linkedToysResult =
            await mediator.Send(new ListToysQuery(Page: 1, PageSize: 10, UserId: userIdOrError.Value));

        if (linkedToysResult.IsError)
            return Problem(linkedToysResult.Errors);

        if (linkedToysResult.Value.Items.All(t => t.MacAddress != macAddress))
            return Problem(Errors.Toy.NotLinkedToUser);

        var ws = webSocketConnectionManager.GetSocket(macAddress);

        if (ws is null)
            return Problem(Errors.Toy.TurnedOff);

        await ws.SendCommand(new SensorDataWs.WearableCommandRequest<DisconnectWifi>(
            WearableCommandOptions.DisconnectWifi,
            new DisconnectWifi { MacAddress = macAddress }
        ));
        
        return Ok();
    }
    

    [HttpGet("{macAddress}/readings")]
    public async Task<IActionResult> GetReadings(string macAddress, [FromQuery] ListSensorReadingsRequest request)
    {
        var query = new ListSensorReadingsQuery(
            macAddress,
            request.From,
            request.To,
            request.Metric,
            request.MinValue,
            request.MaxValue,
            request.Page,
            request.PageSize,
            request.SortBy);
        var result = await mediator.Send(query);
        return result.Match(Ok, Problem);
    }

    [HttpGet("{macAddress}/readings/summary")]
    public async Task<IActionResult> GetReadingsSummaryReport(string macAddress,
        [FromQuery] ListReadingsSummaryReportRequest request)
    {
        if (request.Dummy is true)
        {
            var dummyReport =
                await sensorReadingRepository.GetDailyStatusReportAsyncDummy(macAddress, request.From, request.To);
            return Ok(dummyReport);
        }

        var query = new ListReadingsSummaryReportQuery(macAddress, request.From, request.To);
        var result = await mediator.Send(query);
        return result.Match(Ok, Problem);
    }

    [HttpPost]
    public async Task<IActionResult> CreateToy([FromBody] CreateToyRequest request)
    {
        var command = new CreateToyCommand(request.PatientId, request.Name, request.MacAddress);
        var result = await mediator.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateToy(Guid id, [FromBody] UpdateToyRequest request)
    {
        var command = new UpdateToyCommand(id, request.Name, request.MacAddress);
        var result = await mediator.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteToy(Guid id)
    {
        await toyRepository.HardDeleteAsync(id);
        return Ok();
    }
}