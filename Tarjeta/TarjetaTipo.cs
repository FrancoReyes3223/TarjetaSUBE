namespace TarjetaSUBE
{
    public class TarjetaTipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // FactorTarifa determina que fraccion de la tarifa basica se paga:
        // 1.0m = 100% (Normal), 0.5m = 50% (Medio boleto), 0.0m = 0% (Gratuito / Franquicia completa)
        public decimal FactorTarifa { get; set; } = 1.0m;

        public TarjetaTipo()
        {
        }

        public TarjetaTipo(string nombre, decimal factorTarifa)
        {
            Nombre = nombre;
            FactorTarifa = factorTarifa;
        }

        public int CalcularTarifa(int tarifaBasica)
        {
            return (int)(tarifaBasica * FactorTarifa);
        }

        // Tipos predefinidos para representar los casos pedidos
        public static TarjetaTipo Normal => new TarjetaTipo { Id = 1, Nombre = "Normal", FactorTarifa = 1.0m };
        public static TarjetaTipo MedioBoletoEstudiantil => new TarjetaTipo { Id = 2, Nombre = "Medio boleto estudiantil", FactorTarifa = 0.5m };
        public static TarjetaTipo BoletoGratuitoEstudiantil => new TarjetaTipo { Id = 3, Nombre = "Boleto gratuito estudiantil", FactorTarifa = 0.0m };
        public static TarjetaTipo FranquiciaCompleta => new TarjetaTipo { Id = 4, Nombre = "Franquicia completa", FactorTarifa = 0.0m };
    }
}
