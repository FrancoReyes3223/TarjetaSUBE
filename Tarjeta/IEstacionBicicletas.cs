using System;

namespace TarjetaSUBE
{
    public interface IEstacionBicicletas
    {
        int Id { get; }

        bool RetirarBici(IMedioDePago medioDePago);
        bool RetirarBici(IMedioDePago medioDePago, DateTime fechaHora);
        bool DevolverBici(IMedioDePago medioDePago, DateTime fechaHoraDevolucion);
        decimal CalcularCostoRetiro(IMedioDePago medioDePago, DateTime fechaHora);
        int MultasPendientes(IMedioDePago medioDePago);
        bool PagoTarifaDelDia(IMedioDePago medioDePago, DateTime fechaHora);
        bool TieneBiciEnUso(IMedioDePago medioDePago);
    }
}
