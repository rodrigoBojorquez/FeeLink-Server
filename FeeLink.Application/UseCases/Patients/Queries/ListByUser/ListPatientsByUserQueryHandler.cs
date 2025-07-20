using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Patients.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Queries.ListByUser;

public class ListPatientsByUserQueryHandler(IPatientRepository patientRepository) : IRequestHandler<ListPatientsByUserQuery, ErrorOr<ListResult<PatientResult>>>
{
    public async Task<ErrorOr<ListResult<PatientResult>>> Handle(ListPatientsByUserQuery request, CancellationToken cancellationToken)
    {
        var data = await patientRepository.ListByUserIdAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
        return ListResult<PatientResult>.From(data, data.Items.Select(p => p.ToResult()));
    }
}