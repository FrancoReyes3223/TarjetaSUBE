namespace TarjetaSUBE
{
    public class TarjetaTipo : ITarjetaTipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // PorcentajeTarifa indica el porcentaje de la tarifa basica que se abona:
        // 100 = 100% (Normal), 50 = 50% (Medio boleto), 0 = 0% (Gratuito / Franquicia completa)
        public int PorcentajeTarifa { get; set; } = 100;

        public TarjetaTipo()
        {
        }

        public TarjetaTipo(string nombre, int porcentajeTarifa)
        {
            Nombre = nombre;
            PorcentajeTarifa = porcentajeTarifa;
        }

        public int CalcularTarifa(int tarifaBasica)
        {
            return (tarifaBasica * PorcentajeTarifa) / 100;
        }
    }
}
