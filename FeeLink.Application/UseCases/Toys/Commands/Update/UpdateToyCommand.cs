using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Toys.Commands.Update;

public record UpdateToyCommand(Guid Id, string Name) : IRequest<ErrorOr<Updated>>;