using FluentValidation;

namespace FeeLink.Application.UseCases.Users.Commands.Create;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(50).WithMessage("El nombre no debe exceder los 50 caracteres.");

        RuleFor(x => x.FirstLastName)
            .NotEmpty().WithMessage("El primer apellido es obligatorio.")
            .MaximumLength(50).WithMessage("El primer apellido no debe exceder los 50 caracteres.");

        RuleFor(x => x.SecondLastName)
            .MaximumLength(50).WithMessage("El segundo apellido no debe exceder los 50 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("Debe proporcionar un correo electrónico válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("El rol es obligatorio.");
    }
}