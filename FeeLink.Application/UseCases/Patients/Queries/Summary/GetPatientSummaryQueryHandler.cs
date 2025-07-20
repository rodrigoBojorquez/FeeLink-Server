using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Patients.Common;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Queries.Summary;

public class GetPatientSummaryQueryHandler(ISensorReadingRepository sensorReadingRepository)
    : IRequestHandler<GetPatientSummaryQuery, ErrorOr<PatientSummaryResult>>
{
    public async Task<ErrorOr<PatientSummaryResult>> Handle(GetPatientSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var data = await sensorReadingRepository.GetPatientSummaryAsync(request.Id, request.Date, cancellationToken);

        // No hay registros de esa fecha
        return data ?? new PatientSummaryResult(request.Id, 0.0M, 0.0M, 0.0M);
    }
}