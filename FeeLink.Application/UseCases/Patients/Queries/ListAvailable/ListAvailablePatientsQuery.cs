using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Patients.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Queries.ListAvailable;

public record ListAvailablePatientsQuery(int Page, int PageSize) : IRequest<ErrorOr<ListResult<PatientResult>>>;