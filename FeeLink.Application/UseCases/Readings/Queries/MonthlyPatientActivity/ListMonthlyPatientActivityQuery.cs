using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Readings.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Readings.Queries.MonthlyPatientActivity;

public record ListMonthlyPatientActivityQuery(Guid TherapistId, int Month) : IRequest<ListResult<MonthlyPatientActivityResult>>;