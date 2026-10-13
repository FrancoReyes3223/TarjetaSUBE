using NUnit.Framework;
using TarjetaSUBE; 

namespace TarjetaSUBE.Tests
{
	public class TarjetaTests
	{
	
		[TestCase(2000)]

		[TestCase(3000)]

		[TestCase(4000)]

		[TestCase(5000)]

		[TestCase(8000)]

		[TestCase(10000)]

		[TestCase(15000)]

		[TestCase(20000)]

		[TestCase(25000)]

		[TestCase(30000)]

		public void RecargarSaldo_MontosValidos_SumaElSaldoYDevuelveTrue(int montoRecarga)
		{
		
			var tarjeta = new Tarjeta();
		
			bool resultado = tarjeta.RecargarSaldo(montoRecarga);

			Assert.That(resultado, Is.True); 
			Assert.That(tarjeta.Saldo, Is.EqualTo(montoRecarga)); 
		}

		[Test] 

		public void RecargarSaldo_MontoInvalido_DevuelveFalse()
		{
			var tarjeta = new Tarjeta();
			bool resultado = tarjeta.RecargarSaldo(1500); 
			Assert.That(resultado, Is.False);
		}

		[Test] 
		public void RecargarSaldo_MontoMayorAlLimite_DevuelveFalse()
		{
			var tarjeta = new Tarjeta();
			bool resultado = tarjeta.RecargarSaldo(45000); 
			Assert.That(resultado, Is.False);
		}

		[Test]
		public void PagarCon_ConSaldoSuficiente_DevuelveTrueGeneraBoletoYDescuentaSaldo()
		{

			var tarjeta = new Tarjeta();
			tarjeta.Id = 1;
			tarjeta.NumeroTarjeta = "6061267008306639";

            tarjeta.RecargarSaldo(3000);

			var colectivo = new Colectivo();

			bool pago = colectivo.PagarCon(tarjeta);

			Assert.That(pago, Is.True);

			Assert.That(tarjeta.Boletos, Has.Count.EqualTo(1));

			Boleto boletoGenerado = tarjeta.Boletos[0];

			Assert.That(boletoGenerado.TarifaBasica, Is.EqualTo(1580));

			Assert.That(boletoGenerado.TarjetaId, Is.EqualTo(tarjeta.Id));

			Assert.That(tarjeta.Saldo, Is.EqualTo(1420));
		}

		[Test]
		public void PagarCon_ConSaldoInSuficiente_DevuelveFalseYNoGeneraBoleto()
		{

			var tarjeta = new Tarjeta();
			tarjeta.Id = 2;
			tarjeta.NumeroTarjeta = "6061267008306632";

			var colectivo = new Colectivo();

			bool pago = colectivo.PagarCon(tarjeta);

			Assert.That(pago, Is.False);

			Assert.That(tarjeta.Boletos, Is.Empty);

			Assert.That(tarjeta.Saldo, Is.EqualTo(0));

		}

		[Test]
		public void PagarCon_VariosViajes_LaTarjetaAcumulaUnBoletoPorViaje()
		{
			var tarjeta = new Tarjeta { Id = 3, NumeroTarjeta = "6061267008306633" };
			tarjeta.RecargarSaldo(5000);

			var colectivo = new Colectivo();

			Assert.That(colectivo.PagarCon(tarjeta), Is.True);
			Assert.That(colectivo.PagarCon(tarjeta), Is.True);
			Assert.That(colectivo.PagarCon(tarjeta), Is.True);
			Assert.That(colectivo.PagarCon(tarjeta), Is.False);

			Assert.That(tarjeta.Boletos, Has.Count.EqualTo(3));
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - 3 * 1580));
		}

		[Test]
		public void PagarCon_FranquiciaCompleta_SiemprePuedePagarInclusoConSaldoCero()
		{
			var tarjeta = new Tarjeta
			{
				Id = 10,
				NumeroTarjeta = "10101010",
				TarjetaTipo = new TarjetaTipo("Franquicia completa", 0)
			};

			var colectivo = new Colectivo();
			bool pago = colectivo.PagarCon(tarjeta);

			Assert.That(pago, Is.True);
			Assert.That(tarjeta.Boletos, Has.Count.EqualTo(1));
			Assert.That(tarjeta.Boletos[0].TarifaBasica, Is.EqualTo(0));
			Assert.That(tarjeta.Saldo, Is.EqualTo(0));
		}

		[Test]
		public void PagarCon_MedioBoletoEstudiantil_MontoDelBoletoEsLaMitadDelNormal()
		{
			var tarjetaNormal = new Tarjeta
			{
				Id = 20,
				NumeroTarjeta = "20202020",
				TarjetaTipo = new TarjetaTipo("Normal", 100)
			};
			tarjetaNormal.RecargarSaldo(2000);

			var tarjetaMedio = new Tarjeta
			{
				Id = 21,
				NumeroTarjeta = "21212121",
				TarjetaTipo = new TarjetaTipo("Medio boleto estudiantil", 50)
			};
			tarjetaMedio.RecargarSaldo(2000);

			var colectivo = new Colectivo();

			Assert.That(colectivo.PagarCon(tarjetaNormal), Is.True);
			Assert.That(colectivo.PagarCon(tarjetaMedio), Is.True);

			Boleto boletoNormal = tarjetaNormal.Boletos[0];
			Boleto boletoMedio = tarjetaMedio.Boletos[0];

			Assert.That(boletoMedio.TarifaBasica, Is.EqualTo(boletoNormal.TarifaBasica / 2));
			Assert.That(boletoMedio.TarifaBasica, Is.EqualTo(790));
			Assert.That(tarjetaMedio.Saldo, Is.EqualTo(2000 - 790));
		}

		[Test]
		public void PagarCon_BoletoGratuitoEstudiantil_GeneraBoletoConMontoCero()
		{
			var tarjeta = new Tarjeta
			{
				Id = 30,
				NumeroTarjeta = "30303030",
				TarjetaTipo = new TarjetaTipo("Boleto gratuito estudiantil", 0)
			};

			var colectivo = new Colectivo();
			bool pago = colectivo.PagarCon(tarjeta);

			Assert.That(pago, Is.True);
			Assert.That(tarjeta.Boletos, Has.Count.EqualTo(1));
			Assert.That(tarjeta.Boletos[0].TarifaBasica, Is.EqualTo(0));
		}

		[Test]
		public void TarjetaTipo_NuevoTipoSinCrearClase_CalculaTarifaCorrectamente()
		{
			var tipoUniversitario = new TarjetaTipo("Medio boleto universitario", 50);
			var tarjeta = new Tarjeta { TarjetaTipo = tipoUniversitario };

			int tarifa = tarjeta.CalcularTarifa(1580);

			Assert.That(tarifa, Is.EqualTo(790));
		}

	}
}