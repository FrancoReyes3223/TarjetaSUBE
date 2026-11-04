namespace TarjetaSUBE
{
    public class ColectivoInterurbano : Colectivo
    {
        public const decimal TarifaInterurbana = 3000m;

        public override decimal Tarifa => TarifaInterurbana;
    }
}
