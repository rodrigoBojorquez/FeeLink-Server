using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Companies.Commands.Update;

public class UpdateCompanyHandler(ICompanyRepository companyRepository)
    : IRequestHandler<UpdateCompanyCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
            var company = await companyRepository.GetByIdAsync(request.Id);

            if (company is null)
                return Errors.Company.NotFound;
            
            company.Name = request.Name;
            company.Rfc = request.Rfc;
            company.PersonContact = request.PersonContact;
            company.Address = request.Address;
            company.PhoneNumber = request.PhoneNumber;
        
            await companyRepository.UpdateAsync(company);
            
            return Result.Updated;
        }
}