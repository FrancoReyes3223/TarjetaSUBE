using System;
using System.Collections.Generic;
using System.Linq;

namespace TarjetaSUBE
{
    public class HistorialViajes
    {
        private readonly IReadOnlyList<Boleto> boletos;

        public HistorialViajes(IReadOnlyList<Boleto> boletos)
        {
            this.boletos = boletos;
        }

        public int ViajesDelMes(DateTime fechaHora)
        {
            return boletos.Count(b => b.FechayHora.Year == fechaHora.Year && b.FechayHora.Month == fechaHora.Month);
        }

        public int ViajesDelDia(DateTime fechaHora)
        {
            return boletos.Count(b => b.FechayHora.Date == fechaHora.Date && !b.EsTrasbordo);
        }

        public bool PasaronMinutosDesdeUltimoViaje(DateTime fechaHora, int minutos)
        {
            if (boletos.Count == 0)
            {
                return true;
            }

            Boleto ultimoBoleto = boletos[boletos.Count - 1];
            TimeSpan tiempoPasado = fechaHora - ultimoBoleto.FechayHora;

            return tiempoPasado.TotalMinutes >= minutos;
        }

        public bool EsTrasbordo(string linea, DateTime fechaHora)
        {
            if (boletos.Count == 0 || string.IsNullOrEmpty(linea))
            {
                return false;
            }

            if (fechaHora.DayOfWeek == DayOfWeek.Sunday || fechaHora.Hour < 7 || fechaHora.Hour >= 22)
            {
                return false;
            }

            Boleto ultimoBoleto = boletos[boletos.Count - 1];
            if (string.IsNullOrEmpty(ultimoBoleto.LineaColectivo) || ultimoBoleto.LineaColectivo == linea)
            {
                return false;
            }

            Boleto primerBoleto = boletos.LastOrDefault(b => !b.EsTrasbordo);
            if (primerBoleto == null)
            {
                return false;
            }

            TimeSpan tiempoDesdeElPrimero = fechaHora - primerBoleto.FechayHora;
            return tiempoDesdeElPrimero >= TimeSpan.Zero && tiempoDesdeElPrimero.TotalMinutes <= 60;
        }
    }
}
