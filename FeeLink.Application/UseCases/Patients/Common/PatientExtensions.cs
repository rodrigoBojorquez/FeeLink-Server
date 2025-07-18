using FeeLink.Domain.Entities;

namespace FeeLink.Application.UseCases.Patients.Common;

public static class PatientExtensions
{
    public static PatientResult ToResult(this Patient patient)
    {
        return new PatientResult(
            Id: patient.Id,
            Name: patient.Name,
            LastName: patient.LastName,
            Age: patient.Age,
            Gender: patient.Gender,
            Height: patient.Height,
            Weight: patient.Weight
        );
    }
}