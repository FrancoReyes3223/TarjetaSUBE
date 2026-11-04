using System;

namespace TarjetaSUBE
{
    public class Normal() : TarjetaTipo("Normal")
    {
        public override int PorcentajeTarifa => 100;

        public override decimal CalcularTarifa(decimal tarifaBasica, HistorialViajes historial, DateTime fechaHora)
        {
            int numeroDeViaje = historial.ViajesDelMes(fechaHora) + 1;

            if (numeroDeViaje >= 80)
            {
                return tarifaBasica * 0.75m;
            }

            if (numeroDeViaje >= 30)
            {
                return tarifaBasica * 0.80m;
            }

            return tarifaBasica;
        }

        public override bool PuedeViajar(HistorialViajes historial, DateTime fechaHora)
        {
            return true;
        }
    }
}
