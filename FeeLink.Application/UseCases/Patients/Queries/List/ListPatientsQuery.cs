using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Patients.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Queries.List;

public record ListPatientsQuery(int Page = 1, int PageSize = 10, Guid? UserId = null)
    : IRequest<ListResult<PatientResult>>;