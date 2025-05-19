using ErrorOr;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Companies.Queries.View;

public record ViewCompanyQuery(
    Guid Id
    ) : IRequest<ErrorOr<Company>>; 