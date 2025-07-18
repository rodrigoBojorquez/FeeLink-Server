using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Commands.AssignTherapist;

public class AssignTherapistCommandHandler(IPatientRepository patientRepository)
    : IRequestHandler<AssignTherapistCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(AssignTherapistCommand request, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.IncludeAssignmentsAsync(request.PatientId, cancellationToken);

        if (patient is null)
            return Errors.Patient.NotFound;

        await patientRepository.AssignTherapistsAsync(request.PatientId, request.Therapists, cancellationToken);

        return Result.Created;
    }
}