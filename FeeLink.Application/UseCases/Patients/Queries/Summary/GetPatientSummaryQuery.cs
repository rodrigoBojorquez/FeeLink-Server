using ErrorOr;
using FeeLink.Application.UseCases.Patients.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Queries.Summary;

public record GetPatientSummaryQuery(Guid Id, DateOnly Date) : IRequest<ErrorOr<PatientSummaryResult>>;