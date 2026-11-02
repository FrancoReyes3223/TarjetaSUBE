using System;

namespace TarjetaSUBE
{
    public interface IMedioDePago
    {
        int Id { get; }
        string NombreTipo { get; }
        int Saldo { get; }

        bool PuedeViajar(DateTime fechaHora);
        int CalcularTarifa(int tarifaBasica, DateTime fechaHora);
        bool PuedePagar(int monto);
        void DescontarSaldo(int monto);
        void RegistrarViaje(Boleto boleto);
    }
}
