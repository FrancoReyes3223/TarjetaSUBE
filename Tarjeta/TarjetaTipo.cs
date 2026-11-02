using System;

namespace TarjetaSUBE
{
    public abstract class TarjetaTipo(string nombre) : ITarjetaTipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = nombre;

        public abstract int PorcentajeTarifa { get; }

        public int CalcularTarifa(int tarifaBasica)
        {
            return (tarifaBasica * PorcentajeTarifa) / 100;
        }

        public virtual int CalcularTarifa(int tarifaBasica, HistorialViajes historial, DateTime fechaHora)
        {
            return CalcularTarifa(tarifaBasica);
        }

        public virtual bool PuedeViajar(HistorialViajes historial, DateTime fechaHora)
        {
            return true;
        }
    }
}
