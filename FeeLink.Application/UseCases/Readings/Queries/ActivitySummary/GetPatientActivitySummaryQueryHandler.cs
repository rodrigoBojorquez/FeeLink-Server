using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Readings.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Readings.Queries.ActivitySummary;

public class GetPatientActivitySummaryQueryHandler(ISensorReadingRepository sensorReadingRepository) : IRequestHandler<GetPatientActivitySummaryQuery, PatientActivitySummaryResult>
{
    public async Task<PatientActivitySummaryResult> Handle(GetPatientActivitySummaryQuery request, CancellationToken cancellationToken)
    {
        // Use the repository to get the patient activity count
        var result = await sensorReadingRepository.GetPatientActivityCountAsync(request.Date);

        // Return the result
        return result;
    }
}