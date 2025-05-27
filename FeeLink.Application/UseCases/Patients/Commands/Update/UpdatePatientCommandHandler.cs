using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Commands.Update;

public class UpdatePatientCommandHandler(IPatientRepository patientRepository)
    : IRequestHandler<UpdatePatientCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetByIdAsync(request.Id);

        if (patient is null)
            return Errors.Patient.NotFound;


        patient.Name = request.Name;
        patient.LastName = request.LastName;
        patient.Age = request.Age;
        patient.Gender = request.Gender;
        patient.Height = request.Height;
        patient.Weight = request.Weight;

        await patientRepository.UpdateAsync(patient);

        return Result.Updated;
    }
}