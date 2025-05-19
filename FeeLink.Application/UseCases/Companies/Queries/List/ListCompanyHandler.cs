using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Companies.Queries.List;

public class ListCompanyHandler(ICompanyRepository companyRepository)
    : IRequestHandler<ListCompanyQuery, ErrorOr<ListResult<Company>>>
{
    public async Task<ErrorOr<ListResult<Company>>> Handle(ListCompanyQuery query, CancellationToken cancellationToken)
    {
        var data = await companyRepository.ListAsync(query.Page, query.PageSize,
            u => query.Name != null &&
                 u.Name.ToLower().Contains(query.Name.ToLower())
        );

        return ListResult<Company>.From(data, data.Items.Select(i => i));
    }
}