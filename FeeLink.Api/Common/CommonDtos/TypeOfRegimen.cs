namespace FeeLink.Api.Common.CommonDtos;

public class TypeOfRegimen
{

        public static readonly Dictionary<string, string> Regimenes = new()
        {
            // Personas Físicas
            { "RESICO_PF", "Régimen Simplificado de Confianza (Personas Físicas)" },
            { "ACT_EMP_PROF", "Actividades Empresariales y Profesionales" },
            { "ARRENDAMIENTO", "Arrendamiento de Bienes Inmuebles" },
            { "AGAPES_PF", "Actividades Agrícolas, Ganaderas, Silvícolas y Pesqueras (Personas Físicas)" },
            { "SUELDOS_SALARIOS", "Sueldos y Salarios e Ingresos Asimilados a Salarios" },
            { "ENAJENACION_BIENES", "Enajenación de Bienes" },
            { "ADQUISICION_BIENES", "Adquisición de Bienes" },
            { "INTERESES", "Intereses" },
            { "DIVIDENDOS", "Dividendos" },
            { "PREMIOS", "Ingresos por Premios" },
            { "DEMÁS_INGRESOS", "Demás Ingresos" },
            { "RIF", "Régimen de Incorporación Fiscal (Obsoleto)" },

            // Personas Morales
            { "RESICO_PM", "Régimen Simplificado de Confianza (Personas Morales)" },
            { "GENERAL", "Régimen General de Ley" },
            { "NO_LUCRATIVAS", "Personas Morales con Fines No Lucrativos" },
            { "AGAPES_PM", "Personas Morales del Sector Primario" }
        };

}

