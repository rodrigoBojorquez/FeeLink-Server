namespace FeeLink.Application.UseCases.Patients.Common;

public record PatientSummaryResult(
    Guid Id,
    decimal StablePercentage,
    decimal AnxiousPercentage,
    decimal CrisisPercentage);