using System;

namespace TarjetaSUBE
{
    public interface IColectivo
    {
        int Id { get; }
        string Linea { get; }
        decimal Tarifa { get; }

        bool PagarCon(ITarjetaTransporte medioDePago);
        bool PagarCon(ITarjetaTransporte medioDePago, DateTime fechaHora);
    }
}
