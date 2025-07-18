using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Users.Common;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Users.Queries.GetData;

public class GetUserDataQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserDataQuery, ErrorOr<UserDataResult>>
{
    public async Task<ErrorOr<UserDataResult>> Handle(GetUserDataQuery request, CancellationToken cancellationToken)
    {
        var userData = await userRepository.GetDataAsync(request.Id, cancellationToken);
        if (userData is null)
            return Errors.User.NotFound;

        return userData;
    }
}