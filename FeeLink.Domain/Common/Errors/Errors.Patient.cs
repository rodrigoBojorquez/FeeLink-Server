using ErrorOr;

namespace FeeLink.Domain.Common.Errors;

public static partial class Errors
{
    public static class Patient
    {
        public static Error NotFound => Error.NotFound(
            code: "Patient.NotFound",
            description: "Paciente no encontrado.");

        public static Error MaximumTherapistsReached => Error.Validation(
            code: "Patient.MaximumTherapistsReached",
            description: "El paciente ya tiene el máximo de terapeutas asignados.");
        
        public static Error MaximumTutorsReached => Error.Validation(
            code: "Patient.MaximumTutorsReached",
            description: "El paciente ya tiene el máximo de tutores asignados.");
    }
}