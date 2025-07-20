using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Patients.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Queries.ListAvailable;

public class ListAvailablePatientsQueryHandler(IPatientRepository patientRepository) : IRequestHandler<ListAvailablePatientsQuery, ErrorOr<ListResult<PatientResult>>>
{
    public async Task<ErrorOr<ListResult<PatientResult>>> Handle(ListAvailablePatientsQuery request, CancellationToken cancellationToken)
    {
        var data = await patientRepository.ListAvailableAsync(
            page: request.Page,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);
        var results = data.Items.Select(p => p.ToResult()).ToList();
        return ListResult<PatientResult>.From(data, results);
    }
}