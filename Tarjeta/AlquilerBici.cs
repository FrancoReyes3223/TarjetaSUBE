using System;

namespace TarjetaSUBE
{
    public class AlquilerBici
    {
        public const int MinutosMaximosDeUso = 60;

        public int Id { get; set; }
        public int TarjetaId { get; set; }
        public DateTime FechaRetiro { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public decimal MontoAbonado { get; set; }
        public bool MultaCobrada { get; set; }

        public bool EstaEnCurso()
        {
            return FechaDevolucion == null;
        }

        public bool ExcedioTiempoMaximo()
        {
            if (FechaDevolucion == null)
            {
                return false;
            }

            TimeSpan tiempoDeUso = FechaDevolucion.Value - FechaRetiro;
            return tiempoDeUso.TotalMinutes > MinutosMaximosDeUso;
        }

        public bool TieneMultaPendiente()
        {
            return ExcedioTiempoMaximo() && !MultaCobrada;
        }
    }
}
