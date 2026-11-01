using System;

namespace TarjetaSUBE
{
    public class Normal() : TarjetaTipo("Normal")
    {
        public override int PorcentajeTarifa => 100;

        public override int CalcularTarifa(int tarifaBasica, HistorialViajes historial, DateTime fechaHora)
        {
            int numeroDeViaje = historial.ViajesDelMes(fechaHora) + 1;

            if (numeroDeViaje >= 80)
            {
                return tarifaBasica * 75 / 100;
            }

            if (numeroDeViaje >= 30)
            {
                return tarifaBasica * 80 / 100;
            }

            return tarifaBasica;
        }
    }
}
