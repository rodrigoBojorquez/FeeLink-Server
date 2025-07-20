using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Patients.Commands.AssignTherapist;
using FeeLink.Application.UseCases.Patients.Commands.AssignTutor;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Commands.Create;

public class CreatePatientCommandHandler(IPatientRepository patientRepository, IMediator mediator)
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
        };

        await patientRepository.InsertAsync(patient);

        List<Error> errors = [];

        if (request.TherapistIds is not null && request.TherapistIds.Count > 0)
        {
            var result = await mediator.Send(new AssignTherapistCommand(patient.Id, request.TherapistIds), cancellationToken);
            if (result.IsError)
                errors.AddRange(result.Errors);
        }

        if (request.TutorsIds is not null && request.TutorsIds.Count > 0)
        {
            var result = await mediator.Send(new AssignTutorCommand(patient.Id, request.TutorsIds), cancellationToken);
            if (result.IsError)
                errors.AddRange(result.Errors);
        }

        if (errors.Count > 0)
            return errors;

        return Result.Created;
    }
}