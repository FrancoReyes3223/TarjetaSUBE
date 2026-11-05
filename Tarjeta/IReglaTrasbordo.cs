using System;

namespace TarjetaSUBE
{
    public interface IReglaTrasbordo
    {
        bool EsTrasbordo(IHistorialViajes historial, string linea, DateTime fechaHora);
    }
}
