using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Commands.AssignTherapist;

public record AssignTherapistCommand(Guid PatientId, List<Guid> Therapists) : IRequest<ErrorOr<Created>>;