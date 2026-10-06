namespace TarjetaSUBE
{
    public interface IColectivo
    {
        int Id { get; }
        string Linea { get; }

        Boleto PagarCon(Tarjeta tarjeta);

    }
}