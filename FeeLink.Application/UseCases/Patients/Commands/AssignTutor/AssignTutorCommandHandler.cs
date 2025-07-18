using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Commands.AssignTutor;

public class AssignTutorCommandHandler(IPatientRepository patientRepository)
    : IRequestHandler<AssignTutorCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(AssignTutorCommand request, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.IncludeAssignmentsAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Errors.Patient.NotFound;
        await patientRepository.AssignTutorsAsync(request.PatientId, request.Tutors, cancellationToken);
        return Result.Created;
    }
}