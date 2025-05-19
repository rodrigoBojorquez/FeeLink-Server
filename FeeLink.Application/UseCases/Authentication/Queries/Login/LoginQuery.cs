using ErrorOr;
using FeeLink.Application.UseCases.Authentication.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Authentication.Queries.Login;

public record LoginQuery(
    string Email,
    string Password) : IRequest<ErrorOr<AuthResult>>;