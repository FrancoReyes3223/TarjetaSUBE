namespace TarjetaSUBE
{
    public interface ITarjetaTipo
    {
        int Id { get; }
        string Nombre { get; }
        int PorcentajeTarifa { get; }
        int CalcularTarifa(int tarifaBasica);
    }
}
