using FluentValidation;

namespace FeeLink.Application.UseCases.Users.Queries.ListUsers;

public class ListUsersQueryValidator : AbstractValidator<ListUsersQuery>
{
    public ListUsersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("La página debe ser mayor o igual a 1.");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("El texto de búsqueda no debe exceder los 100 caracteres.");

        RuleFor(x => x.InstitutionId)
            .NotEmpty().When(x => x.InstitutionId.HasValue)
            .WithMessage("El ID de la institución debe ser un GUID válido.");

        RuleFor(x => x.RoleId)
            .NotEmpty().When(x => x.RoleId.HasValue)
            .WithMessage("El ID del rol debe ser un GUID válido.");
    }
}