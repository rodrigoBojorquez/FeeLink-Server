using ErrorOr;

namespace FeeLink.Domain.Common.Errors;

public static partial class Errors
{
    public static class Company
    {
        public static Error NotFound =>
            Error.NotFound("Company.NotFound", "Empresa no encontrada");

        public static Error NotOwner =>
            Error.Forbidden("Company.NotOwner", "Usted no es propietario de la empresa");
    }
}