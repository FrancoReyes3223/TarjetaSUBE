using System;

namespace TarjetaSUBE
{
    public interface IBoleto
    {
        int Id { get; }
        DateTime FechayHora { get; }
        int TarifaBasica { get; }
        int TarjetaId { get; }
        string TipoTarjeta { get; }
        string LineaColectivo { get; }
        int TotalAbonado { get; }
        int SaldoRestante { get; }

    }

}
