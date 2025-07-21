using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Toys.Commands.Update;

public class UpdateToyCommandHandler(IToyRepository toyRepository) : IRequestHandler<UpdateToyCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateToyCommand request, CancellationToken cancellationToken)
    {
        var toy = await toyRepository.GetByIdAsync(request.Id);

        if (toy is null)
            return Errors.Toy.NotFound;
        
        toy.Name = request.Name;
        toy.MacAddress = request.MacAddress;

        await toyRepository.UpdateAsync(toy);
        return Result.Updated;
    }
}