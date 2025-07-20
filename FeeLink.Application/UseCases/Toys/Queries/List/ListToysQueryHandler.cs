using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Toys.Common;
using FeeLink.Application.UseCases.Toys.Queries.ListLinkedWithUser;
using MediatR;

namespace FeeLink.Application.UseCases.Toys.Queries.List;

public class ListToysQueryHandler(
    IToyRepository toyRepository,
    IUserRepository userRepository)
    : IRequestHandler<ListToysQuery, ErrorOr<ListResult<ToyResult>>>
{
    public async Task<ErrorOr<ListResult<ToyResult>>> Handle(ListToysQuery request,
        CancellationToken cancellationToken)
    {
        var toys = await toyRepository.ListAsync(request.Page, request.PageSize, request.UserId, cancellationToken);

        return ListResult<ToyResult>.From(toys, toys.Items.Select(t => t.ToResult()));
    }
}