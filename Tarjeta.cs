namespace TarjetaSUBE;

//En Tarjeta.cs, crear el método de recargar saldo. Debe verificar que el monto esté en
//la lista permitida (2000, 3000, etc.) y que el saldo final no supere los $40.000.
// : (2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000)
public class Tarjeta : ITarjeta
{
    public int NumeroTarjeta { get; set; }
    public int Saldo { get; private set; }

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

}