using System;
using System.Collections.Generic;

namespace TarjetaSUBE
{
    public interface ITarjeta
    {
        int Id { get; }
        string NumeroTarjeta { get; }
        int Saldo { get; }
        int SaldoPendiente { get; }
        int TarjetaTipoId { get; }
        TarjetaTipo? TarjetaTipo { get; }
        List<Boleto> Boletos { get; }

        bool EsMedioBoleto();
        bool TieneLimiteDeViajesDiarios();
        bool RecargarSaldo(int recarga);
        void AcreditarCarga();
        bool PuedePagar(int monto);
        void DescontarSaldo(int saldoAdescontar);
        int CalcularTarifa(int tarifaBasica);
        int CalcularTarifa(int tarifaBasica, DateTime fechaHora);
        bool PasaronCincoMinutos(DateTime fechaHora);
    }
}
