using ErrorOr;

namespace FeeLink.Domain.Common.Errors;

public static partial class Errors
{
    public static class Asset
    {
        public static Error NotFound => 
            Error.NotFound("Asset.NotFound", "El recurso solicitado no fue encontrado.");
        
        public static Error InvalidContentType =>
            Error.Validation("Asset.InvalidContentType", "El tipo de archivo no es válido.");
        
        public static Error InvalidSize =>
            Error.Validation("Asset.InvalidSize", "El tamaño del archivo excede el límite permitido.");
    }
}