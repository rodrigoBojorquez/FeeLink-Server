using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Toys.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Toys.Queries.ListLinkedWithUser;

public record ListLinkedToysWithUserQuery(Guid UserId) : IRequest<ErrorOr<ListResult<ToyResult>>>;