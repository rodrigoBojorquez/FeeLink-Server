using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Common.Errors;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Toys.Commands.Create;

public class CreateToyCommandHandler(IToyRepository toyRepository, IPatientRepository patientRepository) : IRequestHandler<CreateToyCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateToyCommand request, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetByIdAsync(request.PatientId);
        if (patient is null)
            return Errors.Patient.NotFound;
        var hasToy = await patientRepository.HasToyAsync(request.PatientId, cancellationToken);
        if (hasToy)
            return Errors.Toy.PatientAlreadyHasOne;
        
        var toy = new Toy
        {
            PatientId = request.PatientId,
            Name = request.Name,
            MacAddress = request.MacAddress
        };
        
        await toyRepository.InsertAsync(toy);
        
        return Result.Created;
    }
}