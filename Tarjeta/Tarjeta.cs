using System;
using System.Collections.Generic;
using System.Linq;

namespace TarjetaSUBE 
{ 

    // : (2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000)
    public class Tarjeta : ITarjeta
    {
        public const int SaldoNegativoPermitido = -2000;

        public int Id { get; set; }
        public string NumeroTarjeta { get; set; }
        public int Saldo { get; private set; }

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

        public int CalcularTarifa(int tarifaBasica, DateTime fechaHora) //nuevo metodo overload de calcularTarifa, teniendo en cuenta el DateTime
        {
            if (EsMedioBoleto)
            {
                int viajesHoy = 0;
                foreach (var boleto in Boletos)
                {
                    if (boleto.FechayHora.Date == fechaHora.Date)
                    {
                        viajesHoy++;

                        if (viajesHoy >= 2)
                            return tarifaBasica;

                    }
                }
            }
            return CalcularTarifa(tarifaBasica);
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
            if (Saldo + recarga > 40000)
            {
                return false;
            }
            Saldo += recarga;
            return true;
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
        }

        public bool EsMedioBoleto
        {
            get
            {
                return TarjetaTipo != null && TarjetaTipo.PorcentajeTarifa == 50;
            }
        }

        public bool PasaronCincoMinutos(DateTime fechaHora)
        {
            if (!EsMedioBoleto || Boletos.Count == 0)
            {
                return true;
            }

            Boleto ultimoBoleto = Boletos[Boletos.Count - 1];
            TimeSpan tiempoPasado = fechaHora - ultimoBoleto.FechayHora;

            return tiempoPasado.TotalMinutes >= 5;
        }

       
    }
}