using System;

namespace TarjetaSUBE
{
    public interface IHistorialViajes
    {
        int ViajesDelMes(DateTime fechaHora);
        int ViajesDelDia(DateTime fechaHora);
        bool PasaronMinutosDesdeUltimoViaje(DateTime fechaHora, int minutos);
        IBoleto? UltimoBoleto();
        IBoleto? UltimoViajePagado();
    }
}
