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

            if (tarjeta.PuedePagar(tarifa))
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
    }
}

