using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Readings.Common;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Readings.Queries.List;

public record ListSensorReadingsQuery(
    string? MacAddress = null,
    DateTime? From = null, 
    DateTime? To = null,
    Metric? Metric = null,
    float? MinValue = null,
    float? MaxValue = null,
    int Page = 1,
    int PageSize = 50,
    string? SortBy = null) : IRequest<ErrorOr<ListResult<ReadingResult>>>;