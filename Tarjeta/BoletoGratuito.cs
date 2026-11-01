namespace TarjetaSUBE
{
    public class BoletoGratuito() : FranquiciaConLimiteDiario("Boleto gratuito estudiantil")
    {
        public override int PorcentajeTarifa => 0;
    }
}
