using System;

namespace TarjetaSUBE
{
    public class MedioBoleto() : FranquiciaConLimiteDiario("Medio boleto estudiantil")
    {
        public const int MinutosEntreViajes = 5;

        public override int PorcentajeTarifa => 50;

        public override bool PuedeViajar(HistorialViajes historial, DateTime fechaHora)
        {
            return base.PuedeViajar(historial, fechaHora) && historial.PasaronMinutosDesdeUltimoViaje(fechaHora, MinutosEntreViajes);
        }
    }
}
