using ErrorOr;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using MediatR;

namespace FeeLink.Application.UseCases.Companies.Commands.Create;

public record CreateCompanyHandler : IRequestHandler<CreateCompanyCommand, ErrorOr<Created>>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IAuthService _authService;

    public CreateCompanyHandler(ICompanyRepository companyRepository, IAuthService authService)
    {
        _companyRepository = companyRepository;
        _authService = authService;
    }

    public async Task<ErrorOr<Created>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var userId = _authService.GetUserId();

        if (userId.IsError)
            return userId.Errors;
        
        var company = new Domain.Entities.Company
        {
            Name = request.Name,
            Address = request.Address,
            Rfc = request.Rfc,
            PersonContact = request.PersonContact,
            PhoneNumber = request.PhoneNumber
        };
        
        await _companyRepository.InsertAsync(company);

        
        return Result.Created;
    }
}