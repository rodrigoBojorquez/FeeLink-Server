using FeeLink.Api.Common.Controllers;
using FeeLink.Application.UseCases.Readings.Queries.List;
using FeeLink.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FeeLink.Api.Controllers;

public class ReadingsController(IMediator mediator) : ApiController
{
    public record ListReadingsRequest(
        int? Page,
        int? PageSize,
        DateTime? From,
        DateTime? To,
        Metric? Metric,
        float? MinValue,
        float? MaxValue);
    
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListReadingsRequest request)
    {
        var query = new ListSensorReadingsQuery(
            Page: request.Page ?? 1,
            PageSize: request.PageSize ?? 50,
            From: request.From,
            To: request.To,
            Metric: request.Metric,
            MinValue: request.MinValue,
            MaxValue: request.MaxValue);
        
        var result = await mediator.Send(query);
        return result.Match(Ok, Problem);
    }
    
    
}