using System;

namespace TarjetaSUBE
{
    public class Boleto : IBoleto
    {
        public int Id { get; set; }
        public DateTime FechayHora { get; set; }
        public int TarifaBasica { get; set; }
        public int TarjetaId { get; set; }
        public string TipoTarjeta { get; set; } = string.Empty;
        public string LineaColectivo { get; set; } = string.Empty;
        public int TotalAbonado { get; set; }
        public int SaldoRestante { get; set; }
    }
}
