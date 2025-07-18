using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Readings.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Readings.Queries.SummaryReport;

public class ListReadingsSummaryReportQueryHandler(ISensorReadingRepository sensorReadingRepository) : IRequestHandler<ListReadingsSummaryReportQuery, ErrorOr<StatusReportResult>>
{
    public async Task<ErrorOr<StatusReportResult>> Handle(ListReadingsSummaryReportQuery request, CancellationToken cancellationToken)
    {
        return await sensorReadingRepository.GetDailyStatusReportAsync(
            macAddress: request.MacAddress,
            from: request.From,
            to: request.To);
    }
}