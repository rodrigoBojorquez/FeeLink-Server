using ErrorOr;

namespace FeeLink.Domain.Common.Errors;

public partial class Errors
{
    public static class Role
    {
        public static Error NotFound => 
            Error.NotFound("Role.NotFound", "Rol no encontrado");
        
        public static Error NotAllowed =>
            Error.Forbidden("Role.NotAllowed", "No tienes permisos para realizar esta acción");
        
    }
}