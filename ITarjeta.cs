namespace TarjetaSUBE;

public interface ITarjeta
{
    public int NumeroTarjeta { get; }
    public int Saldo { get; private set; }


}