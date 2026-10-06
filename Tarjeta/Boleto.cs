namespace TarjetaSUBE;

public class Boleto : IBoleto
{
    public int Id { get; set; }
    public DateTime FechayHora { get; set; }
    public int TarifaBasica { get; set; }
    public int TarjetaId { get; set; }
}
