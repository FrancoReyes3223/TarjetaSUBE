using System;

namespace TarjetaSUBE { 

    public class Colectivo : IColectivo
    {
        public int Id { get; set; }
        public string Linea { get; set; }

        public bool PagarCon(Tarjeta tarjeta)
        {
            int tarifaBasica = 1580;
            int tarifa = tarjeta.CalcularTarifa(tarifaBasica);

            if (tarjeta.PuedePagar(tarifa) && tarjeta.LimiteDeTiempo?quetengo que fijarme en la lista?)
            {
                tarjeta.DescontarSaldo(tarifa);

                Boleto nuevoBoleto = new Boleto
                {
                    TarifaBasica = tarifaBasica,
                    FechayHora = DateTime.Now,
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

            int tarifa = tarjeta.CalcularTarifa(1580, fechaHora);

            if (tarjeta.PuedePagar(tarifa))
            {
                tarjeta.DescontarSaldo(tarifa);

                Boleto nuevoBoleto = new Boleto
                {
                    TarifaBasica = tarifa,
                    FechayHora = fechaHora,
                    TarjetaId = tarjeta.Id
                };

                tarjeta.Boletos.Add(nuevoBoleto);
                return true;
            }

            return false;
        }
    }
}

