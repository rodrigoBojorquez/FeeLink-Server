using ErrorOr;
using FeeLink.Application.UseCases.Users.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Users.Queries.GetData;

public record GetUserDataQuery(Guid Id) : IRequest<ErrorOr<UserDataResult>>;