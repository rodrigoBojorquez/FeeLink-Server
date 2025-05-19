using ErrorOr;

namespace FeeLink.Domain.Common.Errors;

public static partial class Errors
{
    public class Authentication
    {
        public static Error InvalidCredentials =>
            Error.Unauthorized(code: "Authentication.InvalidCredentials", description: "Credenciales invalidas");
        
        public static Error InvalidToken => 
            Error.Unauthorized(code: "Authentication.InvalidToken", description: "Token invalido");
        
        public static Error InvalidRefreshToken => 
            Error.Unauthorized("User.InvalidRefreshToken", "Refresh Token inválido.");
        
        public static Error TokenExpired =>
            Error.Unauthorized(code: "Authentication.TokenExpired", description: "Token expirado");
        
        public static Error TokenValidationFailed =>
            Error.Failure(code: "Authentication.TokenValidationFailed", description: "Fallo al validar token");
        
        public static Error NotAuthorized =>
            Error.Forbidden(code: "Authentication.NotAuthorized", description: "No autorizado");
        
        public static Error NotAuthenticated =>
            Error.Unauthorized(code: "Authentication.NotAuthenticated", description: "No autenticado");
        
        public static Error MissingRefreshToken =>
            Error.Unauthorized("User.MissingRefreshToken", "Refresh Token no encontrado.");
    }
}