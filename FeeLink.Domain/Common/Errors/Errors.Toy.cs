using ErrorOr;

namespace FeeLink.Domain.Common.Errors;

public static partial class Errors
{
    public static class Toy
    {
        public static Error NotFound => Error.NotFound(
            code: "Toy.NotFound",
            description: "El juguete no fue encontrado"
        );

        public static Error AlreadyExists => Error.Conflict(
            code: "Toy.AlreadyExists",
            description: "El juguete ya existe"
        );

        public static Error InvalidData => Error.Validation(
            code: "Toy.InvalidData",
            description: "Los datos del juguete son inválidos"
        );
        
        public static Error TurnedOff => Error.Failure(
            code: "Toy.TurnedOff",
            description: "El juguete está apagado"
        );
    }
}