using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Commands.AssignTutor;

public record AssignTutorCommand(Guid PatientId, List<Guid> Tutors) : IRequest<ErrorOr<Created>>;