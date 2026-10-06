using System;

namespace TarjetaSUBE
{
    public interface IBoleto
    {
        int Id { get; }
        DateTime FechayHora { get; }
        int TarifaBasica { get; }
        int TarjetaId { get; }

    }

}