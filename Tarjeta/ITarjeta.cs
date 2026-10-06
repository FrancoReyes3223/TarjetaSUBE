namespace TarjetaSUBE
{
    public interface ITarjeta
    {
        int Id { get; }
        string NumeroTarjeta { get; }
        int Saldo { get; }
        bool RecargarSaldo(int recarga);
        void DescontarSaldo(int saldoAdescontar);

    }
}