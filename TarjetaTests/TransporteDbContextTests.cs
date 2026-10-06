using System.Linq;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	// Ejemplo de como testear contra la base en memoria heredando de TestBase.
	public class TransporteDbContextTests : TestBase
	{
		[Test]
		public void Tarjetas_GuardarTarjeta_SePuedeRecuperarPorId()
		{
			var tarjeta = new Tarjeta { NumeroTarjeta = "6061267008306639" };
			tarjeta.RecargarSaldo(2000);

			Db.Tarjetas.Add(tarjeta);
			Db.SaveChanges();

			var guardada = Db.Tarjetas.Find(tarjeta.Id);

			Assert.That(guardada, Is.Not.Null);
			Assert.That(guardada!.NumeroTarjeta, Is.EqualTo("6061267008306639"));
			Assert.That(guardada.Saldo, Is.EqualTo(2000));
		}

		[Test]
		public void Boletos_GuardarBoletoDePagarCon_QuedaAsociadoALaTarjeta()
		{
			var tarjeta = new Tarjeta { NumeroTarjeta = "6061267008306632" };
			tarjeta.RecargarSaldo(3000);
			Db.Tarjetas.Add(tarjeta);
			Db.SaveChanges();

			var boleto = new Colectivo().PagarCon(tarjeta);
			Db.Boletos.Add(boleto);
			Db.SaveChanges();

			var boletosDeLaTarjeta = Db.Boletos.Where(b => b.TarjetaId == tarjeta.Id).ToList();

			Assert.That(boletosDeLaTarjeta, Has.Count.EqualTo(1));
			Assert.That(boletosDeLaTarjeta[0].TarifaBasica, Is.EqualTo(1580));
		}

		[Test]
		public void Modelo_BoletoTieneClaveForaneaATarjeta()
		{
			var claveForanea = Db.Model.FindEntityType(typeof(Boleto))!.GetForeignKeys().Single();

			Assert.That(claveForanea.PrincipalEntityType.ClrType, Is.EqualTo(typeof(Tarjeta)));
			Assert.That(claveForanea.Properties.Single().Name, Is.EqualTo(nameof(Boleto.TarjetaId)));
		}

		[Test]
		public void BaseEnMemoria_CadaTestEmpiezaVacia()
		{
			Assert.That(Db.Tarjetas.Count(), Is.EqualTo(0));
			Assert.That(Db.Boletos.Count(), Is.EqualTo(0));
		}
	}
}
