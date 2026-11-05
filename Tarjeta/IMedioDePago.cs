namespace TarjetaSUBE
{
    public interface IMedioDePago
    {
        int Id { get; }
        decimal Saldo { get; }

        bool PuedePagar(decimal monto);
        void DescontarSaldo(decimal monto);
    }
}
