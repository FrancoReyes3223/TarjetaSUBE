namespace TarjetaSUBE
{
    public interface ITarjetaRecargable
    {
        int Saldo { get; }
        int SaldoPendiente { get; }

        bool RecargarSaldo(int recarga);
    }
}
