using System;

namespace TarjetaSUBE
{
    public interface ITarjetaTipo
    {
        int Id { get; }
        string Nombre { get; }
        int PorcentajeTarifa { get; }
        int CalcularTarifa(int tarifaBasica);
        int CalcularTarifa(int tarifaBasica, HistorialViajes historial, DateTime fechaHora);
        bool PuedeViajar(HistorialViajes historial, DateTime fechaHora);
    }
}
