using System;
using System.Linq;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	public class ColectivoInterurbanoTests : TestBase
	{
		private readonly DateTime martesALas10 = new DateTime(2026, 10, 27, 10, 0, 0);

		[Test]
		public void Tarifa_ColectivoUrbano_EsLaTarifaBasica()
		{
			var colectivo = new Colectivo();

			Assert.That(colectivo.Tarifa, Is.EqualTo(1580));
		}

		[Test]
		public void Tarifa_ColectivoInterurbano_EsLaTarifaInterurbana()
		{
			var colectivo = new ColectivoInterurbano();

			Assert.That(colectivo.Tarifa, Is.EqualTo(3000));
		}

		[Test]
		public void PagarCon_TarjetaNormal_AbonaTarifaInterurbana()
		{
			var tarjeta = new Tarjeta
			{
				Id = 50,
				TarjetaTipo = new TarjetaTipo("Normal", 100)
			};
			tarjeta.RecargarSaldo(5000);

			var colectivo = new ColectivoInterurbano { Linea = "Galvez" };

			bool pago = colectivo.PagarCon(tarjeta, martesALas10);

			Assert.That(pago, Is.True);

			Boleto boleto = tarjeta.Boletos[0];

			Assert.That(boleto.LineaColectivo, Is.EqualTo("Galvez"));
			Assert.That(boleto.TarifaBasica, Is.EqualTo(3000));
			Assert.That(boleto.TotalAbonado, Is.EqualTo(3000));
			Assert.That(boleto.SaldoRestante, Is.EqualTo(5000 - 3000));
			Assert.That(tarjeta.Saldo, Is.EqualTo(2000));
		}

		[Test]
		public void PagarCon_TarjetaSinTipo_AbonaTarifaInterurbana()
		{
			var tarjeta = new Tarjeta { Id = 51 };
			tarjeta.RecargarSaldo(3000);

			bool pago = new ColectivoInterurbano { Linea = "Baigorria" }.PagarCon(tarjeta, martesALas10);

			Assert.That(pago, Is.True);
			Assert.That(tarjeta.Boletos[0].TipoTarjeta, Is.EqualTo("Normal"));
			Assert.That(tarjeta.Boletos[0].TotalAbonado, Is.EqualTo(3000));
			Assert.That(tarjeta.Saldo, Is.EqualTo(0));
		}

		[Test]
		public void PagarCon_MedioBoleto_AbonaLaMitadDeLaTarifaInterurbana()
		{
			var tarjeta = new Tarjeta
			{
				Id = 52,
				TarjetaTipo = new TarjetaTipo("Medio boleto estudiantil", 50)
			};
			tarjeta.RecargarSaldo(2000);

			bool pago = new ColectivoInterurbano { Linea = "Galvez" }.PagarCon(tarjeta, martesALas10);

			Assert.That(pago, Is.True);
			Assert.That(tarjeta.Boletos[0].TarifaBasica, Is.EqualTo(3000));
			Assert.That(tarjeta.Boletos[0].TotalAbonado, Is.EqualTo(1500));
			Assert.That(tarjeta.Saldo, Is.EqualTo(2000 - 1500));
		}

		[TestCase("Boleto gratuito estudiantil")]
		[TestCase("Franquicia completa")]
		public void PagarCon_FranquiciaGratuita_NoAbonaNada(string nombreTipo)
		{
			var tarjeta = new Tarjeta
			{
				Id = 53,
				TarjetaTipo = new TarjetaTipo(nombreTipo, 0)
			};

			bool pago = new ColectivoInterurbano { Linea = "Galvez" }.PagarCon(tarjeta, martesALas10);

			Assert.That(pago, Is.True);
			Assert.That(tarjeta.Boletos[0].TipoTarjeta, Is.EqualTo(nombreTipo));
			Assert.That(tarjeta.Boletos[0].TarifaBasica, Is.EqualTo(3000));
			Assert.That(tarjeta.Boletos[0].TotalAbonado, Is.EqualTo(0));
			Assert.That(tarjeta.Saldo, Is.EqualTo(0));
		}

		[Test]
		public void PagarCon_SinSaldo_SuperaElSaldoNegativoPermitidoYNoGeneraBoleto()
		{
			var tarjeta = new Tarjeta { Id = 54 };

			bool pago = new ColectivoInterurbano().PagarCon(tarjeta, martesALas10);

			Assert.That(pago, Is.False);
			Assert.That(tarjeta.Boletos, Is.Empty);
			Assert.That(tarjeta.Saldo, Is.EqualTo(0));
		}

		[Test]
		public void PagarCon_SaldoInsuficiente_PermiteQuedarConSaldoNegativo()
		{
			var tarjeta = new Tarjeta { Id = 55 };
			tarjeta.RecargarSaldo(2000);

			bool pago = new ColectivoInterurbano().PagarCon(tarjeta, martesALas10);

			Assert.That(pago, Is.True);
			Assert.That(tarjeta.Saldo, Is.EqualTo(-1000));
			Assert.That(tarjeta.Boletos[0].SaldoRestante, Is.EqualTo(-1000));
		}

		[Test]
		public void PagarCon_MedioBoleto_NoPermiteViajarAntesDePasarCincoMinutos()
		{
			var tarjeta = new Tarjeta { TarjetaTipo = new TarjetaTipo("Medio boleto estudiantil", 50) };
			tarjeta.RecargarSaldo(10000);

			var colectivo = new ColectivoInterurbano();

			Assert.That(colectivo.PagarCon(tarjeta, martesALas10), Is.True);
			Assert.That(colectivo.PagarCon(tarjeta, martesALas10.AddMinutes(3)), Is.False);
			Assert.That(colectivo.PagarCon(tarjeta, martesALas10.AddMinutes(6)), Is.True);
			Assert.That(tarjeta.Boletos, Has.Count.EqualTo(2));
		}

		[Test]
		public void PagarCon_MedioBoleto_TercerViajeDelDiaAbonaTarifaInterurbanaCompleta()
		{
			var tarjeta = new Tarjeta { TarjetaTipo = new TarjetaTipo("Medio boleto estudiantil", 50) };
			tarjeta.RecargarSaldo(10000);

			var colectivo = new ColectivoInterurbano();

			colectivo.PagarCon(tarjeta, martesALas10);
			colectivo.PagarCon(tarjeta, martesALas10.AddMinutes(10));
			colectivo.PagarCon(tarjeta, martesALas10.AddMinutes(20));

			Assert.That(tarjeta.Boletos[0].TotalAbonado, Is.EqualTo(1500));
			Assert.That(tarjeta.Boletos[1].TotalAbonado, Is.EqualTo(1500));
			Assert.That(tarjeta.Boletos[2].TotalAbonado, Is.EqualTo(3000));
			Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 1500 - 1500 - 3000));
		}

		[Test]
		public void PagarCon_BoletoGratuito_TercerViajeDelDiaAbonaTarifaInterurbanaCompleta()
		{
			var tarjeta = new Tarjeta { TarjetaTipo = new TarjetaTipo("Boleto gratuito estudiantil", 0) };
			tarjeta.RecargarSaldo(5000);

			var colectivo = new ColectivoInterurbano();

			colectivo.PagarCon(tarjeta, martesALas10);
			colectivo.PagarCon(tarjeta, martesALas10.AddMinutes(10));
			colectivo.PagarCon(tarjeta, martesALas10.AddMinutes(20));

			Assert.That(tarjeta.Boletos[2].TotalAbonado, Is.EqualTo(3000));
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - 3000));
		}

		[Test]
		public void PagarCon_UrbanoEInterurbano_CadaBoletoTieneSuTarifa()
		{
			var tarjeta = new Tarjeta();
			tarjeta.RecargarSaldo(10000);

			new Colectivo { Linea = "K" }.PagarCon(tarjeta, martesALas10);
			new ColectivoInterurbano { Linea = "Galvez" }.PagarCon(tarjeta, martesALas10.AddMinutes(30));

			Assert.That(tarjeta.Boletos[0].TotalAbonado, Is.EqualTo(1580));
			Assert.That(tarjeta.Boletos[1].TotalAbonado, Is.EqualTo(3000));
			Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 1580 - 3000));
		}

		[Test]
		public void Colectivos_GuardarColectivoInterurbano_SeRecuperaComoInterurbano()
		{
			Db.Colectivos.Add(new Colectivo { Linea = "K" });
			Db.Colectivos.Add(new ColectivoInterurbano { Linea = "Galvez" });
			Db.SaveChanges();

			var interurbanos = Db.Colectivos.OfType<ColectivoInterurbano>().ToList();

			Assert.That(Db.Colectivos.Count(), Is.EqualTo(2));
			Assert.That(interurbanos, Has.Count.EqualTo(1));
			Assert.That(interurbanos[0].Linea, Is.EqualTo("Galvez"));
			Assert.That(interurbanos[0].Tarifa, Is.EqualTo(3000));
		}
	}
}
