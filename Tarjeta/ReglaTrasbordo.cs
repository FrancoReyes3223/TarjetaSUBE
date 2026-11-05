using System;

namespace TarjetaSUBE
{
    public class ReglaTrasbordo : IReglaTrasbordo
    {
        public const int MinutosMaximosDesdeElPrimerViaje = 60;
        public const int HoraInicio = 7;
        public const int HoraFin = 22;

        public bool EsTrasbordo(IHistorialViajes historial, string linea, DateTime fechaHora)
        {
            if (string.IsNullOrEmpty(linea) || !EstaEnFranjaHoraria(fechaHora))
            {
                return false;
            }

            IBoleto? ultimoBoleto = historial.UltimoBoleto();
            if (ultimoBoleto == null || string.IsNullOrEmpty(ultimoBoleto.LineaColectivo) || ultimoBoleto.LineaColectivo == linea)
            {
                return false;
            }

            IBoleto? primerViaje = historial.UltimoViajePagado();
            if (primerViaje == null)
            {
                return false;
            }

            TimeSpan tiempoDesdeElPrimero = fechaHora - primerViaje.FechayHora;
            return tiempoDesdeElPrimero >= TimeSpan.Zero && tiempoDesdeElPrimero.TotalMinutes <= MinutosMaximosDesdeElPrimerViaje;
        }

        private static bool EstaEnFranjaHoraria(DateTime fechaHora)
        {
            if (fechaHora.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            return fechaHora.Hour >= HoraInicio && fechaHora.Hour < HoraFin;
        }
    }
}
