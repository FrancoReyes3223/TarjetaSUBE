using System;

namespace TarjetaSUBE
{
    public interface IMedioDePago
    {
        int Id { get; }
        string NombreTipo { get; }
        decimal Saldo { get; }

        bool PuedeViajar(DateTime fechaHora);
        decimal CalcularTarifa(decimal tarifaBasica, DateTime fechaHora);
        bool PuedePagar(decimal monto);
        void DescontarSaldo(decimal monto);
        void RegistrarViaje(Boleto boleto);
    }
}
