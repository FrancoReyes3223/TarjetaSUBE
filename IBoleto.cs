namespace TarjetaSUBE;

public interface IBoleto
{
    public int Id { get; }
    public int FechayHora { get; private set; }
    public int TarifaBasica { get; private set } 


}