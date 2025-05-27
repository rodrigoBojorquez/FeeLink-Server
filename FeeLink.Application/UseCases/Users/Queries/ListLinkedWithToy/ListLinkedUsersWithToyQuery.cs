using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Users.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Users.Queries.ListLinkedWithToy;

public record ListLinkedUsersWithToyQuery(Guid ToyId) : IRequest<ErrorOr<ListResult<UserResult>>>;