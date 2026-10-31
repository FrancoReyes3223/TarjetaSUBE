using System;
using System.Collections.Generic;
using System.Linq;

namespace TarjetaSUBE 
{ 
    public class Tarjeta : ITarjeta
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

        public Tarjeta()
        {
            Saldo = 0; 
        }

        public Tarjeta(TarjetaTipo tipo) : this()
        {
            TarjetaTipo = tipo;
            TarjetaTipoId = tipo.Id;
        }

        public int CalcularTarifa(int tarifaBasica, DateTime fechaHora)
        {
            if (EsNormal())
            {
                return CalcularTarifaUsoFrecuente(tarifaBasica, fechaHora);
            }

            if (TieneLimiteDeViajesDiarios())
            {
                int viajesHoy = 0;
                foreach (var boleto in Boletos)
                {
                    if (boleto.FechayHora.Date == fechaHora.Date)
                    {
                        viajesHoy++;

                        if (viajesHoy >= 2)
                        {
                            return tarifaBasica;
                        }
                    }
                }
            }
            return CalcularTarifa(tarifaBasica);
        }

        public int CalcularTarifaUsoFrecuente(int tarifaBasica, DateTime fechaHora)
        {
            int numeroDeViaje = ViajesDelMes(fechaHora) + 1;

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

        public int ViajesDelMes(DateTime fechaHora)
        {
            return Boletos.Count(b => b.FechayHora.Year == fechaHora.Year && b.FechayHora.Month == fechaHora.Month);
        }

        public int CalcularTarifa(int tarifaBasica)
        {
            if (TarjetaTipo != null)
            {
                return TarjetaTipo.CalcularTarifa(tarifaBasica);
            }
            return tarifaBasica;
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
            return TarjetaTipo == null || TarjetaTipo.PorcentajeTarifa == 100;
        }

        public bool EsMedioBoleto()
        {
            if (TarjetaTipo != null && TarjetaTipo.PorcentajeTarifa == 50)
            {
                return true;
            }

            return false;
        }

        public bool TieneLimiteDeViajesDiarios()
        {
            if (TarjetaTipo == null)
            {
                return false;
            }

            switch (TarjetaTipo.PorcentajeTarifa)
            {
                case 50:
                    return true;

                case 0:
                    string nombre = TarjetaTipo.Nombre.Trim().ToLower();
                    return (nombre != "franquicia completa");

                default:
                    return false;
            }
        }

        public bool PasaronCincoMinutos(DateTime fechaHora)
        {
            if (!EsMedioBoleto() || Boletos.Count == 0)
            {
                return true;
            }

            Boleto ultimoBoleto = Boletos[Boletos.Count - 1];
            TimeSpan tiempoPasado = fechaHora - ultimoBoleto.FechayHora;

            return tiempoPasado.TotalMinutes > 5;
        }
    }
}
