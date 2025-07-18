using FeeLink.Application.Common.Extensions;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Patients.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Queries.List;

public class ListPatientsQueryHandler(IPatientRepository patientRepository)
    : IRequestHandler<ListPatientsQuery, ListResult<PatientResult>>
{
    public async Task<ListResult<PatientResult>> Handle(ListPatientsQuery request, CancellationToken cancellationToken)
    {
        var data = await patientRepository.ListAsync(request.Page, request.PageSize);

        return new ListResult<PatientResult>(data.Items.Select(p => p.ToResult()), data.TotalItems, data.Page,
            data.TotalItems.GetTotalPages(data.PageSize ?? 1));
    }
}