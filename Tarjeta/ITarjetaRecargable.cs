namespace TarjetaSUBE
{
    public interface ITarjetaRecargable
    {
        decimal Saldo { get; }
        decimal SaldoPendiente { get; }

        bool RecargarSaldo(decimal recarga);
    }
}
