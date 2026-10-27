using System;

namespace TarjetaSUBE
{
    public interface IColectivo
    {
        int Id { get; }
        string Linea { get; }

        bool PagarCon(Tarjeta tarjeta);
        bool PagarCon(Tarjeta tarjeta, DateTime fechaHora);
    }
}
