using System;

namespace TarjetaSUBE
{
    public class TarjetaTipo : ITarjetaTipo
    {
        public const int HoraInicioFranquicias = 6;
        public const int HoraFinFranquicias = 22;
        public const int ViajeInicioPrimerDescuento = 30;
        public const int ViajeInicioSegundoDescuento = 80;
        public const decimal FactorPrimerDescuento = 0.80m;
        public const decimal FactorSegundoDescuento = 0.75m;

        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int PorcentajeTarifa { get; set; } = 100;
        public bool EsFranquicia { get; set; }
        public int? ViajesDiariosConFranquicia { get; set; }
        public int? MinutosEntreViajes { get; set; }

        public static TarjetaTipo Normal()
        {
            return new TarjetaTipo { Nombre = "Normal" };
        }

        public static TarjetaTipo MedioBoleto()
        {
            return new TarjetaTipo
            {
                Nombre = "Medio boleto estudiantil",
                PorcentajeTarifa = 50,
                EsFranquicia = true,
                ViajesDiariosConFranquicia = 2,
                MinutosEntreViajes = 5
            };
        }

        public static TarjetaTipo BoletoGratuito()
        {
            return new TarjetaTipo
            {
                Nombre = "Boleto gratuito estudiantil",
                PorcentajeTarifa = 0,
                EsFranquicia = true,
                ViajesDiariosConFranquicia = 2
            };
        }

        public static TarjetaTipo FranquiciaCompleta()
        {
            return new TarjetaTipo
            {
                Nombre = "Franquicia completa",
                PorcentajeTarifa = 0,
                EsFranquicia = true
            };
        }

        public decimal CalcularTarifa(decimal tarifaBasica)
        {
            return (tarifaBasica * PorcentajeTarifa) / 100m;
        }

        public decimal CalcularTarifa(decimal tarifaBasica, HistorialViajes historial, DateTime fechaHora)
        {
            if (!EsFranquicia)
            {
                return CalcularTarifa(tarifaBasica) * FactorUsoFrecuente(historial.ViajesDelMes(fechaHora) + 1);
            }

            if (ViajesDiariosConFranquicia.HasValue && historial.ViajesDelDia(fechaHora) >= ViajesDiariosConFranquicia.Value)
            {
                return tarifaBasica;
            }

            return CalcularTarifa(tarifaBasica);
        }

        public bool PuedeViajar(HistorialViajes historial, DateTime fechaHora)
        {
            if (!EsFranquicia)
            {
                return true;
            }

            if (!EstaEnFranjaHoraria(fechaHora))
            {
                return false;
            }

            return !MinutosEntreViajes.HasValue || historial.PasaronMinutosDesdeUltimoViaje(fechaHora, MinutosEntreViajes.Value);
        }

        private static decimal FactorUsoFrecuente(int numeroDeViaje)
        {
            if (numeroDeViaje >= ViajeInicioSegundoDescuento)
            {
                return FactorSegundoDescuento;
            }

            if (numeroDeViaje >= ViajeInicioPrimerDescuento)
            {
                return FactorPrimerDescuento;
            }

            return 1m;
        }

        private static bool EstaEnFranjaHoraria(DateTime fechaHora)
        {
            if (fechaHora.DayOfWeek == DayOfWeek.Saturday || fechaHora.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            return fechaHora.Hour >= HoraInicioFranquicias && fechaHora.Hour < HoraFinFranquicias;
        }
    }
}
