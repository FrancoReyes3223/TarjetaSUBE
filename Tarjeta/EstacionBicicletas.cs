using System;
using System.Collections.Generic;
using System.Linq;

namespace TarjetaSUBE
{
    public class EstacionBicicletas : IEstacionBicicletas
    {
        public const decimal TarifaDiaria = 2418.75m;
        public const decimal ValorMulta = 1250m;

        public int Id { get; set; }
        public List<AlquilerBici> Alquileres { get; set; } = new List<AlquilerBici>();

        public bool TieneBiciEnUso(IMedioDePago medioDePago)
        {
            return Alquileres.Any(a => a.TarjetaId == medioDePago.Id && a.EstaEnCurso());
        }

        public int MultasDelDia(IMedioDePago medioDePago, DateTime fechaHora)
        {
            return Alquileres.Count(a =>
                a.TarjetaId == medioDePago.Id &&
                a.FechaRetiro.Date == fechaHora.Date &&
                a.ExcedioTiempoMaximo());
        }

        public decimal CalcularCostoRetiro(IMedioDePago medioDePago, DateTime fechaHora)
        {
            int multas = MultasDelDia(medioDePago, fechaHora);
            return TarifaDiaria + (multas * ValorMulta);
        }

        public bool RetirarBici(IMedioDePago medioDePago)
        {
            return RetirarBici(medioDePago, DateTime.Now);
        }

        public bool RetirarBici(IMedioDePago medioDePago, DateTime fechaHora)
        {
            if (TieneBiciEnUso(medioDePago))
            {
                return false;
            }

            decimal costo = CalcularCostoRetiro(medioDePago, fechaHora);

            if (!medioDePago.PuedePagar(costo))
            {
                return false;
            }

            medioDePago.DescontarSaldo(costo);

            AlquilerBici nuevoAlquiler = new AlquilerBici
            {
                TarjetaId = medioDePago.Id,
                FechaRetiro = fechaHora,
                MontoAbonado = costo
            };

            Alquileres.Add(nuevoAlquiler);
            return true;
        }

        public bool DevolverBici(IMedioDePago medioDePago, DateTime fechaHoraDevolucion)
        {
            AlquilerBici alquilerActivo = Alquileres.LastOrDefault(a => a.TarjetaId == medioDePago.Id && a.EstaEnCurso());

            if (alquilerActivo == null || fechaHoraDevolucion < alquilerActivo.FechaRetiro)
            {
                return false;
            }

            alquilerActivo.FechaDevolucion = fechaHoraDevolucion;
            return true;
        }
    }
}
