using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Readings.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Readings.Queries.MonthlyPatientActivity;

public class ListMonthlyPatientActivityQueryHandler(ISensorReadingRepository sensorReadingRepository) : IRequestHandler<ListMonthlyPatientActivityQuery, ListResult<MonthlyPatientActivityResult>>
{
    public async Task<ListResult<MonthlyPatientActivityResult>> Handle(ListMonthlyPatientActivityQuery request, CancellationToken cancellationToken)
    {
        // Use the repository to get the monthly patient activity summary
        var result = await sensorReadingRepository.GetTherapistPatientActivitySummaryAsync(request.TherapistId, request.Month);

        // Return the result
        return result;
    }
}