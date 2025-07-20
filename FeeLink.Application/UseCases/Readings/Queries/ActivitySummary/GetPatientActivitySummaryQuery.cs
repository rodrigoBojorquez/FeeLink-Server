using FeeLink.Application.UseCases.Readings.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Readings.Queries.ActivitySummary;

public record GetPatientActivitySummaryQuery(DateOnly Date) : IRequest<PatientActivitySummaryResult>;