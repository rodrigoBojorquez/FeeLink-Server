using FluentValidation;

namespace FeeLink.Application.UseCases.Readings.Queries.MonthlyPatientActivity;

public class ListMonthlyPatientActivityQueryValidator : AbstractValidator<ListMonthlyPatientActivityQuery>
{
    public ListMonthlyPatientActivityQueryValidator()
    {
        RuleFor(x => x.TherapistId)
            .NotEmpty().WithMessage("El ID del terapeuta no puede estar vacío.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("El mes debe estar entre 1 y 12.");
    }
}