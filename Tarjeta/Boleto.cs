using System;

namespace TarjetaSUBE
{
    public class Boleto : IBoleto
    {
        public int Id { get; set; }
        public DateTime FechayHora { get; set; }
        public decimal TarifaBasica { get; set; }
        public int TarjetaId { get; set; }
        public string TipoTarjeta { get; set; } = string.Empty;
        public string LineaColectivo { get; set; } = string.Empty;
        public decimal TotalAbonado { get; set; }
        public decimal SaldoRestante { get; set; }
        public bool EsTrasbordo { get; set; }
    }
}
