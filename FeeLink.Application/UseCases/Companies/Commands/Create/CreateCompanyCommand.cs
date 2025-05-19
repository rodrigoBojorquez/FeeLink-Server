using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Companies.Commands.Create;

public record CreateCompanyCommand(
    string Name,
    string Address,
    string Rfc,
    string PersonContact,
    string PhoneNumber
) : IRequest<ErrorOr<Created>>; 