using System.Security.Claims;
using FeeLink.Api.Common.Controllers;
using FeeLink.Application.UseCases.Companies.Commands.Create;
using FeeLink.Application.UseCases.Companies.Commands.Delete;
using FeeLink.Application.UseCases.Companies.Commands.Update;
using FeeLink.Application.UseCases.Companies.Queries.List;
using FeeLink.Application.UseCases.Companies.Queries.View;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FeeLink.Api.Controllers;

public class CompaniesController(IMediator mediator)
    : ApiController
{
    public record CreateCompany(
        string Name,
        string Address,
        string Rfc,
        string PersonContact,
        string PhoneNumber
    );

    public record CompleteCompany(
        Guid Id,
        string Name,
        string Address,
        string Rfc,
        string PersonContact,
        string PhoneNumber
    );

    public record ListCompany(
        int? Page = 1,
        int? PageSize = 10,
        string? Search = ""
    );

    [HttpGet()]
    public async Task<IActionResult> GetAllCompanies([FromQuery] ListCompany input)
    {
        var query = new ListCompanyQuery(
            input.Page ?? 1,
            input.PageSize ?? 10,
            input.Search
        );

        var result = await mediator.Send(query);

        return result.Match(
            companies => Ok(companies),
            Problem
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var query = new ViewCompanyQuery(id);

        var result = await mediator.Send(query);

        return result.Match(
            Ok,
            Problem
        );
    }

    [HttpPost()]
    public async Task<IActionResult> CreateExpense([FromBody] CreateCompany request)
    {
        var command = new CreateCompanyCommand(
            request.Name, request.Address, request.Rfc, request.PersonContact, request.PhoneNumber
        );

        var result = await mediator.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExpenseOrIncome(Guid id, [FromForm] CreateCompany request)
    {
        var command = new UpdateCompanyCommand(
            id, request.Name, request.Address, request.Rfc, request.PersonContact, request.PhoneNumber 
        );

        var result = await mediator.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        var command = new DeleteCompanyCommand(id);

        var result = await mediator.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }
}