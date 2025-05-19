using FluentValidation;

namespace FeeLink.Application.UseCases.Authentication.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(command => command.OldPassword)
            .NotEmpty().WithMessage("La contraseña actual es obligatoria.");

        RuleFor(command => command.NewPassword)
            .NotEmpty().WithMessage("La nueva contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La nueva contraseña debe tener al menos 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("La nueva contraseña debe contener al menos una letra mayúscula.")
            .Matches(@"[a-z]").WithMessage("La nueva contraseña debe contener al menos una letra minúscula.");

        RuleFor(command => command.ConfirmPassword)
            .NotEmpty().WithMessage("La confirmación de la nueva contraseña es obligatoria.")
            .Equal(command => command.NewPassword)
            .WithMessage("La confirmación de la contraseña no coincide con la nueva contraseña.");
    }
}