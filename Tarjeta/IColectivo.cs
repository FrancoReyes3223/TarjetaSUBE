using System;

namespace TarjetaSUBE
{
    public interface IColectivo
    {
        int Id { get; }
        string Linea { get; }
        int Tarifa { get; }

        bool PagarCon(IMedioDePago medioDePago);
        bool PagarCon(IMedioDePago medioDePago, DateTime fechaHora);
    }
}
