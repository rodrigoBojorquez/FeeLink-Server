using FluentValidation;

namespace FeeLink.Application.UseCases.Users.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("El ID del usuario es obligatorio.");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(50).WithMessage("El nombre no debe superar los 50 caracteres.")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
            .WithMessage("El nombre solo puede contener letras y espacios.");

        RuleFor(command => command.FirstLastName)
            .MaximumLength(50).WithMessage("El primer apellido no debe superar los 50 caracteres.")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
            .WithMessage("El primer apellido solo puede contener letras y espacios.");

        RuleFor(command => command.SecondLastName)
            .MaximumLength(50).WithMessage("El segundo apellido no debe superar los 50 caracteres.")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$")
            .WithMessage("El segundo apellido solo puede contener letras y espacios.");
    }
}