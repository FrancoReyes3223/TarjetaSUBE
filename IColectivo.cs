namespace TarjetaSUBE;

public interface IColectivo
{
    public int Id { get; }
    public string Linea { get; private set; }

}