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
            return boletos.Count(b => b.FechayHora.Date == fechaHora.Date);
        }

        public bool PasaronMinutosDesdeUltimoViaje(DateTime fechaHora, int minutos)
        {
            if (boletos.Count == 0)
            {
                return true;
            }

            Boleto ultimoBoleto = boletos[boletos.Count - 1];
            TimeSpan tiempoPasado = fechaHora - ultimoBoleto.FechayHora;

            return tiempoPasado.TotalMinutes > minutos;
        }
    }
}
