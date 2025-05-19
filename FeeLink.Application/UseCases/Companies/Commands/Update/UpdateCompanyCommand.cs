using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Companies.Commands.Update;

public record UpdateCompanyCommand(
    Guid Id,
    string Name,
    string Address,
    string Rfc,
    string PersonContact,
    string PhoneNumber
    ) : IRequest<ErrorOr<Updated>>;
