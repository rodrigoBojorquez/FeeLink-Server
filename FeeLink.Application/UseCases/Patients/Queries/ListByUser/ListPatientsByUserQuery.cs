using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Patients.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Queries.ListByUser;

public record ListPatientsByUserQuery(int Page, int PageSize, Guid UserId) : IRequest<ErrorOr<ListResult<PatientResult>>>;