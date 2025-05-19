using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Companies.Queries.List;

public record ListCompanyQuery (
    int Page = 1,
    int PageSize = 10,
    string? Name = null
    ) : IRequest<ErrorOr<ListResult<Company>>>;