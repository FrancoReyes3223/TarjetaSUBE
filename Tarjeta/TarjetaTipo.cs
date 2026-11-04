using System;

namespace TarjetaSUBE
{
    public abstract class TarjetaTipo(string nombre) : ITarjetaTipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = nombre;

        public abstract int PorcentajeTarifa { get; }

        public decimal CalcularTarifa(decimal tarifaBasica)
        {
            return (tarifaBasica * PorcentajeTarifa) / 100m;
        }

        public virtual decimal CalcularTarifa(decimal tarifaBasica, HistorialViajes historial, DateTime fechaHora)
        {
            return CalcularTarifa(tarifaBasica);
        }

        public virtual bool PuedeViajar(HistorialViajes historial, DateTime fechaHora)
        {
            if (fechaHora.DayOfWeek == DayOfWeek.Saturday || fechaHora.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            return fechaHora.Hour >= 6 && fechaHora.Hour < 22;
        }
    }
}
