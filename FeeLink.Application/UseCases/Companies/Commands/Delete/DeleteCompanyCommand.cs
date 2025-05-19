using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Companies.Commands.Delete;

public record DeleteCompanyCommand(
    Guid Id
    ) : IRequest<ErrorOr<Deleted>>;