namespace FeeLink.Application.UseCases.Patients.Common;

public record PatientResult(Guid Id, string Name, string LastName, int Age, string Gender, float Height, float Weight);