using System;

namespace TarjetaSUBE
{
    public interface ITarjetaTipo
    {
        int Id { get; }
        string Nombre { get; }
        int PorcentajeTarifa { get; }
        decimal CalcularTarifa(decimal tarifaBasica);
        decimal CalcularTarifa(decimal tarifaBasica, HistorialViajes historial, DateTime fechaHora);
        bool PuedeViajar(HistorialViajes historial, DateTime fechaHora);
    }
}
