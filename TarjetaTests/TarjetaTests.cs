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
		public void PagarCon_ConSaldoSuficiente_DevuelveBoletoYDescuentaSaldo()
		{
		
			var tarjeta = new Tarjeta();
			tarjeta.Id = 1;
			tarjeta.NumeroTarjeta = "6061267008306639";

            tarjeta.RecargarSaldo(3000);

			var colectivo = new Colectivo(); 

			Boleto boletoGenerado = colectivo.PagarCon(tarjeta);

			Assert.That(boletoGenerado, Is.Not.Null);

			Assert.That(boletoGenerado.TarifaBasica, Is.EqualTo(1580));

			Assert.That(boletoGenerado.TarjetaId, Is.EqualTo(tarjeta.Id));

			Assert.That(tarjeta.Saldo, Is.EqualTo(1420));
		}

		[Test]
		public void PagarCon_ConSaldoInSuficiente_DevuelveNull()
		{

			var tarjeta = new Tarjeta();
			tarjeta.Id = 2;
			tarjeta.NumeroTarjeta = "6061267008306632";

			var colectivo = new Colectivo();

			Boleto boletoGenerado = colectivo.PagarCon(tarjeta);

			Assert.That(boletoGenerado, Is.Null);

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
			Boleto boleto = colectivo.PagarCon(tarjeta);

			Assert.That(boleto, Is.Not.Null);
			Assert.That(boleto.TarifaBasica, Is.EqualTo(0));
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

			Boleto boletoNormal = colectivo.PagarCon(tarjetaNormal);
			Boleto boletoMedio = colectivo.PagarCon(tarjetaMedio);

			Assert.That(boletoNormal, Is.Not.Null);
			Assert.That(boletoMedio, Is.Not.Null);
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
			Boleto boleto = colectivo.PagarCon(tarjeta);

			Assert.That(boleto, Is.Not.Null);
			Assert.That(boleto.TarifaBasica, Is.EqualTo(0));
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