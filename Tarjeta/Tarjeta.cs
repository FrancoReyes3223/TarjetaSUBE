using System;
using System.Collections.Generic;
using System.Linq;

namespace TarjetaSUBE
{
    public class Tarjeta : IMedioDePago, ITarjetaRecargable
    {
        public const decimal SaldoNegativoPermitido = -2000m;
        public const decimal SaldoMaximo = 56000m;

        public int Id { get; set; }
        public string NumeroTarjeta { get; set; }
        public decimal Saldo { get; private set; }
        public decimal SaldoPendiente { get; private set; }

        public int TarjetaTipoId { get; set; }
        public TarjetaTipo? TarjetaTipo { get; set; }

        public List<Boleto> Boletos { get; set; } = new List<Boleto>();

        public string NombreTipo => ObtenerTipo().Nombre;

        public Tarjeta()
        {
            Saldo = 0m;
        }

        public Tarjeta(TarjetaTipo tipo) : this()
        {
            TarjetaTipo = tipo;
            TarjetaTipoId = tipo.Id;
        }

        private TarjetaTipo ObtenerTipo()
        {
            return TarjetaTipo ?? TarjetaTipo.Normal();
        }

        private HistorialViajes ObtenerHistorial()
        {
            return new HistorialViajes(Boletos);
        }

        public decimal CalcularTarifa(decimal tarifaBasica, DateTime fechaHora)
        {
            return ObtenerTipo().CalcularTarifa(tarifaBasica, ObtenerHistorial(), fechaHora);
        }

        public int ViajesDelMes(DateTime fechaHora)
        {
            return ObtenerHistorial().ViajesDelMes(fechaHora);
        }

        public decimal CalcularTarifa(decimal tarifaBasica)
        {
            return ObtenerTipo().CalcularTarifa(tarifaBasica);
        }

        public bool RecargarSaldo(decimal recarga)
        {
            decimal[] montosPermitidos = { 2000m, 3000m, 4000m, 5000m, 8000m, 10000m, 15000m, 20000m, 25000m, 30000m };

            if (!montosPermitidos.Contains(recarga))
            {
                return false;
            }
            SaldoPendiente += recarga;
            AcreditarCarga();
            return true;
        }

        private void AcreditarCarga()
        {
            decimal acreditar = Math.Min(SaldoMaximo - Saldo, SaldoPendiente);

            if (acreditar > 0m)
            {
                Saldo += acreditar;
                SaldoPendiente -= acreditar;
            }
        }

        public bool PuedePagar(decimal monto)
        {
            return Saldo - monto >= SaldoNegativoPermitido;
        }

        public void DescontarSaldo(decimal saldoAdescontar)
        {
            if (!PuedePagar(saldoAdescontar))
                return;

            Saldo -= saldoAdescontar;
            AcreditarCarga();
        }

        public void RegistrarViaje(Boleto boleto)
        {
            Boletos.Add(boleto);
        }

        public bool EsNormal()
        {
            return !ObtenerTipo().EsFranquicia;
        }

        public bool PuedeViajar(DateTime fechaHora)
        {
            return ObtenerTipo().PuedeViajar(ObtenerHistorial(), fechaHora);
        }

        public bool EsTrasbordo(string linea, DateTime fechaHora)
        {
            return ObtenerHistorial().EsTrasbordo(linea, fechaHora);
        }
    }
}
