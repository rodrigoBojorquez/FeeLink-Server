using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Readings.Common;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Readings.Queries.List;

public class ListSensorReadingsQueryHandler(ISensorReadingRepository sensorReadingRepository, IToyRepository toyRepository) : IRequestHandler<ListSensorReadingsQuery, ErrorOr<ListResult<ReadingResult>>>
{
    public async Task<ErrorOr<ListResult<ReadingResult>>> Handle(ListSensorReadingsQuery request, CancellationToken cancellationToken)
    {
        var toy = await toyRepository.GetByMacAsync(request.MacAddress, cancellationToken);
        if (toy is null)
            return Errors.Toy.NotFound;
        
        var result = await sensorReadingRepository.ListAsync(
            macAddress: request.MacAddress,
            from: request.From,
            to: request.To,
            metric: request.Metric,
            minValue: request.MinValue,
            maxValue: request.MaxValue,
            page: request.Page,
            pageSize: request.PageSize,
            sortBy: request.SortBy);

        return result;
    }
}