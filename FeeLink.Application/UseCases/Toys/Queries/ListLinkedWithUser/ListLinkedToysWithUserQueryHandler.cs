using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Toys.Common;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Toys.Queries.ListLinkedWithUser;

public class ListLinkedToysWithUserQueryHandler(
    IToyRepository toyRepository,
    IUserRepository userRepository,
    IPatientRepository patientRepository)
    : IRequestHandler<ListLinkedToysWithUserQuery, ErrorOr<ListResult<ToyResult>>>
{
    public async Task<ErrorOr<ListResult<ToyResult>>> Handle(ListLinkedToysWithUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId);

        if (user is null)
            return Errors.User.NotFound;
        
        var toys = await toyRepository.ListByUserIdAsync(user.Id, cancellationToken);

        return toys;
    }
}