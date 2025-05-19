using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Common.Errors;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Companies.Queries.View;

public class ViewCompanyHandler(ICompanyRepository companyRepository)
    : IRequestHandler<ViewCompanyQuery, ErrorOr<Company>>
{
    public async Task<ErrorOr<Company>> Handle(ViewCompanyQuery request, CancellationToken cancellationToken)
    {
        var data = await companyRepository.GetByIdAsync(request.Id);

        if (data is null)
            return Errors.Company.NotFound;
        
        return data;
    }
}