using System;
using System.Collections.Generic;
using System.Linq;

namespace TarjetaSUBE
{
    public class Tarjeta : IMedioDePago, ITarjetaRecargable
    {
        public const int SaldoNegativoPermitido = -2000;
        public const int SaldoMaximo = 56000;

        public int Id { get; set; }
        public string NumeroTarjeta { get; set; }
        public int Saldo { get; private set; }
        public int SaldoPendiente { get; private set; }

        public int TarjetaTipoId { get; set; }
        public TarjetaTipo? TarjetaTipo { get; set; }

        public List<Boleto> Boletos { get; set; } = new List<Boleto>();

        public string NombreTipo => ObtenerTipo().Nombre;

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
            return TarjetaTipo ?? new Normal();
        }

        private HistorialViajes ObtenerHistorial()
        {
            return new HistorialViajes(Boletos);
        }

        public int CalcularTarifa(int tarifaBasica, DateTime fechaHora)
        {
            return ObtenerTipo().CalcularTarifa(tarifaBasica, ObtenerHistorial(), fechaHora);
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

        private void AcreditarCarga()
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

        public void RegistrarViaje(Boleto boleto)
        {
            Boletos.Add(boleto);
        }

        public bool EsNormal()
        {
            return ObtenerTipo() is Normal;
        }

        public bool PuedeViajar(DateTime fechaHora)
        {
            return ObtenerTipo().PuedeViajar(ObtenerHistorial(), fechaHora);
        }
    }
}
