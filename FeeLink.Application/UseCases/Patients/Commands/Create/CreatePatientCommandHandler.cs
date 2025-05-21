using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Commands.Create;

public class CreatePatientCommandHandler(IPatientRepository patientRepository)
    : IRequestHandler<CreatePatientCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Name = request.Name,
            LastName = request.LastName,
            Age = request.Age,
            Gender = request.Gender,
            Height = request.Height,
            Weight = request.Weight,
            UserId = request.UserId
        };

        await patientRepository.InsertAsync(patient);

        return Result.Created;
    }
}