using System;

namespace TarjetaSUBE
{
    public interface ITarjetaTransporte : IMedioDePago
    {
        string NombreTipo { get; }

        bool PuedeViajar(DateTime fechaHora);
        bool EsTrasbordo(string linea, DateTime fechaHora);
        decimal CalcularTarifa(decimal tarifaBasica, DateTime fechaHora);
        void RegistrarViaje(Boleto boleto);
    }
}
