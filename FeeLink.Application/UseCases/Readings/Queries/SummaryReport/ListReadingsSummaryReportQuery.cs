using ErrorOr;
using FeeLink.Application.UseCases.Readings.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Readings.Queries.SummaryReport;

public record ListReadingsSummaryReportQuery(string MacAddress, DateOnly From, DateOnly To)
    : IRequest<ErrorOr<StatusReportResult>>;