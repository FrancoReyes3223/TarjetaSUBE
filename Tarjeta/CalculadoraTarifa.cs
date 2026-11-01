using System;

namespace TarjetaSUBE
{
    public class CalculadoraTarifa
    {
        public const int ViajesDiariosConFranquicia = 2;

        public int Calcular(TarjetaTipo tipo, int tarifaBasica, HistorialViajes historial, DateTime fechaHora)
        {
            if (tipo.EsNormal())
            {
                return CalcularUsoFrecuente(tarifaBasica, historial.ViajesDelMes(fechaHora) + 1);
            }

            if (tipo.TieneLimiteDeViajesDiarios() && historial.ViajesDelDia(fechaHora) >= ViajesDiariosConFranquicia)
            {
                return tarifaBasica;
            }

            return tipo.CalcularTarifa(tarifaBasica);
        }

        public int CalcularUsoFrecuente(int tarifaBasica, int numeroDeViaje)
        {
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
