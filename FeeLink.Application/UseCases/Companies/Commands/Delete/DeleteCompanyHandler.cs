using ErrorOr;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Companies.Commands.Delete;

public class DeleteCompanyHandler(ICompanyRepository companyRepository, IAuthService authService)
    : IRequestHandler<DeleteCompanyCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var userId = authService.GetUserId();

        if (userId.IsError)
            return userId.Errors;
        
        var company = await companyRepository.GetByIdAsync(request.Id);

        if (company is null)
            return Errors.Company.NotFound;
        
        await companyRepository.HardDeleteAsync(company.Id);
        
        return Result.Deleted;
    }
}