using ErrorOr;

namespace FeeLink.Domain.Common.Errors;

public static partial class Errors
{
    public static class User
    {
        public static Error DuplicateEmail =>
            Error.Conflict(code: "User.DuplicateEmail", description: "Correo electrónico ya registrado");

        public static Error NotFound =>
            Error.NotFound(code: "User.UserNotFound", description: "Usuario no encontrado");

        public static Error NotBelongToInstitution =>
            Error.Forbidden(code: "User.NotBelongToInstitution",
                description: "El usuario no pertenece a la institución");
        
        public static Error ExternalAuthenticationConflict =>
            Error.Conflict(code: "User.ExternalAuthenticationConflict",
                description: "El usuario se registrado con otro método de autenticación");
        
        public static Error NotConnected =>
            Error.Failure(code: "User.NotConnected",
                description: "El usuario no está conectado a la aplicación");
        
        public static Error InvalidId =>
            Error.Validation(code: "User.InvalidId",
                description: "El ID del usuario no es válido");
    }
}