using System;

namespace TarjetaSUBE
{
    public interface ITarjetaTipo
    {
        int Id { get; }
        string Nombre { get; }
        int PorcentajeTarifa { get; }
        bool EsFranquicia { get; }
        int? ViajesDiariosConFranquicia { get; }
        int? MinutosEntreViajes { get; }

        decimal CalcularTarifa(decimal tarifaBasica);
        decimal CalcularTarifa(decimal tarifaBasica, HistorialViajes historial, DateTime fechaHora);
        bool PuedeViajar(HistorialViajes historial, DateTime fechaHora);
    }
}
