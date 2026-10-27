using System;

namespace TarjetaSUBE { 

    public class Colectivo : IColectivo
    {
        public const int TarifaBasica = 1580;

        public int Id { get; set; }
        public string Linea { get; set; }

        public bool PagarCon(Tarjeta tarjeta)
        {
            return PagarCon(tarjeta, DateTime.Now);
        }

        public bool PagarCon(Tarjeta tarjeta, DateTime fechaHora)
        {
            if (!tarjeta.PasaronCincoMinutos(fechaHora))
            {
                return false;
            }

            int tarifa = tarjeta.CalcularTarifa(TarifaBasica, fechaHora);

            if (tarjeta.PuedePagar(tarifa))
            {
                tarjeta.DescontarSaldo(tarifa);

                Boleto nuevoBoleto = new Boleto
                {
                    TarifaBasica = TarifaBasica,
                    FechayHora = fechaHora,
                    TarjetaId = tarjeta.Id,
                    TipoTarjeta = tarjeta.TarjetaTipo?.Nombre ?? "Normal",
                    LineaColectivo = Linea,
                    TotalAbonado = tarifa,
                    SaldoRestante = tarjeta.Saldo
                };

                tarjeta.Boletos.Add(nuevoBoleto);
                return true;
            }

            return false;
        }
    }
}
