namespace TarjetaSUBE
{
    public class ColectivoInterurbano : Colectivo
    {
        public const int TarifaInterurbana = 3000;

        public override int Tarifa => TarifaInterurbana;
    }
}
