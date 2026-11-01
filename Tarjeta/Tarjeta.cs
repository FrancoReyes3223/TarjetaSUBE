using System;
using System.Collections.Generic;
using System.Linq;

namespace TarjetaSUBE
{
    public class Tarjeta : ITarjeta
    {
        public const int SaldoNegativoPermitido = -2000;
        public const int SaldoMaximo = 56000;

        private static readonly CalculadoraTarifa calculadoraTarifa = new CalculadoraTarifa();

        public int Id { get; set; }
        public string NumeroTarjeta { get; set; }
        public int Saldo { get; private set; }
        public int SaldoPendiente { get; private set; }

        public int TarjetaTipoId { get; set; }
        public TarjetaTipo? TarjetaTipo { get; set; }

        public List<Boleto> Boletos { get; set; } = new List<Boleto>();

        public Tarjeta()
        {
            Saldo = 0;
        }

        public Tarjeta(TarjetaTipo tipo) : this()
        {
            TarjetaTipo = tipo;
            TarjetaTipoId = tipo.Id;
        }

        private TarjetaTipo ObtenerTipo()
        {
            return TarjetaTipo ?? new TarjetaTipo("Normal", 100);
        }

        private HistorialViajes ObtenerHistorial()
        {
            return new HistorialViajes(Boletos);
        }

        public int CalcularTarifa(int tarifaBasica, DateTime fechaHora)
        {
            return calculadoraTarifa.Calcular(ObtenerTipo(), tarifaBasica, ObtenerHistorial(), fechaHora);
        }

        public int CalcularTarifaUsoFrecuente(int tarifaBasica, DateTime fechaHora)
        {
            return calculadoraTarifa.CalcularUsoFrecuente(tarifaBasica, ViajesDelMes(fechaHora) + 1);
        }

        public int ViajesDelMes(DateTime fechaHora)
        {
            return ObtenerHistorial().ViajesDelMes(fechaHora);
        }

        public int CalcularTarifa(int tarifaBasica)
        {
            return ObtenerTipo().CalcularTarifa(tarifaBasica);
        }

        public bool RecargarSaldo(int recarga)
        {
            int[] montosPermitidos = { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };

            if (!montosPermitidos.Contains(recarga))
            {
                return false;
            }
            SaldoPendiente += recarga;
            AcreditarCarga();
            return true;
        }

        public void AcreditarCarga()
        {
            int acreditar = Math.Min(SaldoMaximo - Saldo, SaldoPendiente);

            if (acreditar > 0)
            {
                Saldo += acreditar;
                SaldoPendiente -= acreditar;
            }
        }

        public bool PuedePagar(int monto)
        {
            return Saldo - monto >= SaldoNegativoPermitido;
        }

        public void DescontarSaldo(int saldoAdescontar)
        {
            if (!PuedePagar(saldoAdescontar))
                return;

            Saldo -= saldoAdescontar;
            AcreditarCarga();
        }

        public bool EsNormal()
        {
            return ObtenerTipo().EsNormal();
        }

        public bool EsMedioBoleto()
        {
            return ObtenerTipo().EsMedioBoleto();
        }

        public bool TieneLimiteDeViajesDiarios()
        {
            return ObtenerTipo().TieneLimiteDeViajesDiarios();
        }

        public bool PasaronCincoMinutos(DateTime fechaHora)
        {
            return !EsMedioBoleto() || ObtenerHistorial().PasaronMinutosDesdeUltimoViaje(fechaHora, 5);
        }
    }
}
