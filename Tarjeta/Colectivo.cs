using System;

namespace TarjetaSUBE {

    public class Colectivo : IColectivo
    {
        public const decimal TarifaBasica = 1580m;

        public int Id { get; set; }
        public string Linea { get; set; }

        public virtual decimal Tarifa => TarifaBasica;

        public bool PagarCon(IMedioDePago medioDePago)
        {
            return PagarCon(medioDePago, DateTime.Now);
        }

        public bool PagarCon(IMedioDePago medioDePago, DateTime fechaHora)
        {
            if (!medioDePago.PuedeViajar(fechaHora))
            {
                return false;
            }

            bool esTrasbordo = medioDePago.EsTrasbordo(Linea, fechaHora);
            decimal tarifa = esTrasbordo ? 0m : medioDePago.CalcularTarifa(Tarifa, fechaHora);

            if (medioDePago.PuedePagar(tarifa))
            {
                medioDePago.DescontarSaldo(tarifa);

                Boleto nuevoBoleto = new Boleto
                {
                    TarifaBasica = Tarifa,
                    FechayHora = fechaHora,
                    TarjetaId = medioDePago.Id,
                    TipoTarjeta = medioDePago.NombreTipo,
                    LineaColectivo = Linea,
                    TotalAbonado = tarifa,
                    SaldoRestante = medioDePago.Saldo,
                    EsTrasbordo = esTrasbordo
                };

                medioDePago.RegistrarViaje(nuevoBoleto);
                return true;
            }

            return false;
        }
    }
}
