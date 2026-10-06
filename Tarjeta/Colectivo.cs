using System;

namespace TarjetaSUBE { 

    public class Colectivo : IColectivo
    {
        public int Id { get; set; }
        public string Linea { get; set; }

        public Boleto PagarCon(Tarjeta tarjeta)
        {
            int tarifa = 1580;
        
            if (tarjeta.Saldo >= tarifa)
            {
                tarjeta.DescontarSaldo(tarifa);

                Boleto nuevoBoleto = new Boleto
                {
                    TarifaBasica = tarifa,
                    FechayHora = DateTime.Now,
                    TarjetaId = tarjeta.Id  
                };

                return nuevoBoleto; 
            }

            return null;
        }
    }
}

