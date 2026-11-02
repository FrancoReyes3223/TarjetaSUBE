using System;

namespace TarjetaSUBE
{
    public abstract class FranquiciaConLimiteDiario(string nombre) : TarjetaTipo(nombre)
    {
        public const int ViajesDiariosConFranquicia = 2;

        public override int CalcularTarifa(int tarifaBasica, HistorialViajes historial, DateTime fechaHora)
        {
            if (historial.ViajesDelDia(fechaHora) >= ViajesDiariosConFranquicia)
            {
                return tarifaBasica;
            }

            return CalcularTarifa(tarifaBasica);
        }
    }
}
