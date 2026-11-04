using System;

namespace TarjetaSUBE
{
    public interface IBoleto
    {
        int Id { get; }
        DateTime FechayHora { get; }
        decimal TarifaBasica { get; }
        int TarjetaId { get; }
        string TipoTarjeta { get; }
        string LineaColectivo { get; }
        decimal TotalAbonado { get; }
        decimal SaldoRestante { get; }
        bool EsTrasbordo { get; }
    }
}
