using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Toys.Commands.Create;

public class CreateToyCommandHandler(IToyRepository toyRepository) : IRequestHandler<CreateToyCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateToyCommand request, CancellationToken cancellationToken)
    {
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