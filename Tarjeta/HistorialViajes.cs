using System;
using System.Collections.Generic;
using System.Linq;

namespace TarjetaSUBE
{
    public class HistorialViajes : IHistorialViajes
    {
        private readonly IReadOnlyList<IBoleto> boletos;

        public HistorialViajes(IReadOnlyList<IBoleto> boletos)
        {
            this.boletos = boletos;
        }

        public int ViajesDelMes(DateTime fechaHora)
        {
            return boletos.Count(b => b.FechayHora.Year == fechaHora.Year && b.FechayHora.Month == fechaHora.Month && !b.EsTrasbordo);
        }

        public int ViajesDelDia(DateTime fechaHora)
        {
            return boletos.Count(b => b.FechayHora.Date == fechaHora.Date && !b.EsTrasbordo);
        }

        public bool PasaronMinutosDesdeUltimoViaje(DateTime fechaHora, int minutos)
        {
            IBoleto? ultimoBoleto = UltimoBoleto();
            if (ultimoBoleto == null)
            {
                return true;
            }

            TimeSpan tiempoPasado = fechaHora - ultimoBoleto.FechayHora;
            return tiempoPasado.TotalMinutes >= minutos;
        }

        public IBoleto? UltimoBoleto()
        {
            return boletos.Count == 0 ? null : boletos[boletos.Count - 1];
        }

        public IBoleto? UltimoViajePagado()
        {
            return boletos.LastOrDefault(b => !b.EsTrasbordo);
        }
    }
}
