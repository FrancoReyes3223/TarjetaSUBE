using System;
using System.Linq;

namespace TarjetaSUBE 
{ 

    // : (2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000)
    public class Tarjeta : ITarjeta
    {
        public int Id { get; set; }
        public string NumeroTarjeta { get; set; }
        public int Saldo { get; private set; }

        public Tarjeta()
        {
            Saldo = 0; 
        }

        public bool RecargarSaldo(int recarga)
        {
            int[] montosPermitidos = { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };

            if (!montosPermitidos.Contains(recarga))
            {
                return false;
            }
            if (Saldo + recarga > 40000)
            {
                return false;
            }
            Saldo += recarga;
            return true;
        }

        public void DescontarSaldo(int saldoAdescontar)
        {
            if (Saldo < saldoAdescontar) 
                return;

            Saldo -= saldoAdescontar;
        }

    }
}