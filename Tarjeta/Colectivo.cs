using System;

namespace TarjetaSUBE { 

    public class Colectivo : IColectivo
    {
        public const int TarifaBasica = 1580;

        public int Id { get; set; }
        public string Linea { get; set; }

        public virtual int Tarifa => TarifaBasica;

        public bool PagarCon(Tarjeta tarjeta)
        {
            return PagarCon(tarjeta, DateTime.Now);
        }

        public bool PagarCon(Tarjeta tarjeta, DateTime fechaHora)
        {
            if (!tarjeta.PuedeViajar(fechaHora))
            {
                return false;
            }

            int tarifa = tarjeta.CalcularTarifa(Tarifa, fechaHora);

            if (tarjeta.PuedePagar(tarifa))
            {
                tarjeta.DescontarSaldo(tarifa);

                Boleto nuevoBoleto = new Boleto
                {
                    TarifaBasica = Tarifa,
                    FechayHora = fechaHora,
                    TarjetaId = tarjeta.Id,
                    TipoTarjeta = tarjeta.NombreTipo,
                    LineaColectivo = Linea,
                    TotalAbonado = tarifa,
                    SaldoRestante = tarjeta.Saldo
                };

                tarjeta.RegistrarViaje(nuevoBoleto);
                return true;
            }

            return false;
        }
    }
}
