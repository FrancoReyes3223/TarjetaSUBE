using System;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	public class BoletoTests
	{
		[Test]
		public void PagarCon_TarjetaNormal_BoletoTieneTodosLosDatos()
		{
			var tarjeta = new Tarjeta
			{
				Id = 40,
				NumeroTarjeta = "40404040",
				TarjetaTipo = new TarjetaTipo("Normal", 100)
			};
			tarjeta.RecargarSaldo(3000);

			var colectivo = new Colectivo { Linea = "K" };

			DateTime antes = DateTime.Now;
			colectivo.PagarCon(tarjeta);
			DateTime despues = DateTime.Now;

			Boleto boleto = tarjeta.Boletos[0];

			Assert.That(boleto.FechayHora, Is.InRange(antes, despues));
			Assert.That(boleto.TipoTarjeta, Is.EqualTo("Normal"));
			Assert.That(boleto.LineaColectivo, Is.EqualTo("K"));
			Assert.That(boleto.TarifaBasica, Is.EqualTo(1580));
			Assert.That(boleto.TotalAbonado, Is.EqualTo(1580));
			Assert.That(boleto.SaldoRestante, Is.EqualTo(3000 - 1580));
			Assert.That(boleto.TarjetaId, Is.EqualTo(40));
		}

		[Test]
		public void PagarCon_TarjetaSinTipo_BoletoEsNormal()
		{
			var tarjeta = new Tarjeta { Id = 41 };
			tarjeta.RecargarSaldo(2000);

			new Colectivo { Linea = "102" }.PagarCon(tarjeta);

			Boleto boleto = tarjeta.Boletos[0];

			Assert.That(boleto.TipoTarjeta, Is.EqualTo("Normal"));
			Assert.That(boleto.TotalAbonado, Is.EqualTo(1580));
		}

		[Test]
		public void PagarCon_MedioBoletoEstudiantil_BoletoAbonaLaMitad()
		{
			var tarjeta = new Tarjeta
			{
				Id = 42,
				TarjetaTipo = new TarjetaTipo("Medio boleto estudiantil", 50)
			};
			tarjeta.RecargarSaldo(2000);

			new Colectivo { Linea = "K" }.PagarCon(tarjeta);

			Boleto boleto = tarjeta.Boletos[0];

			Assert.That(boleto.TipoTarjeta, Is.EqualTo("Medio boleto estudiantil"));
			Assert.That(boleto.TarifaBasica, Is.EqualTo(1580));
			Assert.That(boleto.TotalAbonado, Is.EqualTo(790));
			Assert.That(boleto.SaldoRestante, Is.EqualTo(2000 - 790));
		}

		[Test]
		public void PagarCon_BoletoGratuitoEstudiantil_BoletoNoAbonaNada()
		{
			var tarjeta = new Tarjeta
			{
				Id = 43,
				TarjetaTipo = new TarjetaTipo("Boleto gratuito estudiantil", 0)
			};

			new Colectivo { Linea = "K" }.PagarCon(tarjeta);

			Boleto boleto = tarjeta.Boletos[0];

			Assert.That(boleto.TipoTarjeta, Is.EqualTo("Boleto gratuito estudiantil"));
			Assert.That(boleto.TarifaBasica, Is.EqualTo(1580));
			Assert.That(boleto.TotalAbonado, Is.EqualTo(0));
			Assert.That(boleto.SaldoRestante, Is.EqualTo(0));
		}

		[Test]
		public void PagarCon_FranquiciaCompleta_BoletoNoAbonaNada()
		{
			var tarjeta = new Tarjeta
			{
				Id = 44,
				TarjetaTipo = new TarjetaTipo("Franquicia completa", 0)
			};

			new Colectivo { Linea = "K" }.PagarCon(tarjeta);

			Boleto boleto = tarjeta.Boletos[0];

			Assert.That(boleto.TipoTarjeta, Is.EqualTo("Franquicia completa"));
			Assert.That(boleto.TarifaBasica, Is.EqualTo(1580));
			Assert.That(boleto.TotalAbonado, Is.EqualTo(0));
			Assert.That(boleto.SaldoRestante, Is.EqualTo(0));
		}

		[Test]
		public void PagarCon_QuedaConSaldoNegativo_BoletoMuestraSaldoRestanteNegativo()
		{
			var tarjeta = new Tarjeta { Id = 45 };

			new Colectivo { Linea = "K" }.PagarCon(tarjeta);

			Boleto boleto = tarjeta.Boletos[0];

			Assert.That(boleto.TotalAbonado, Is.EqualTo(1580));
			Assert.That(boleto.SaldoRestante, Is.EqualTo(-1580));
		}

		[Test]
		public void PagarCon_DespuesDeRecargarConDeuda_BoletoAbonaSoloLaTarifa()
		{
			// La deuda se salda al recargar, no en el viaje siguiente
			var tarjeta = new Tarjeta { Id = 46 };
			var colectivo = new Colectivo { Linea = "K" };

			colectivo.PagarCon(tarjeta);
			tarjeta.RecargarSaldo(3000);
			colectivo.PagarCon(tarjeta);

			Boleto boleto = tarjeta.Boletos[1];

			Assert.That(boleto.TotalAbonado, Is.EqualTo(1580));
			Assert.That(boleto.SaldoRestante, Is.EqualTo(3000 - 1580 - 1580));
		}
	}
}
