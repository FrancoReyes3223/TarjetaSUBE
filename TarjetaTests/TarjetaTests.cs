using System;
using NUnit.Framework;
using TarjetaSUBE; 

namespace TarjetaSUBE.Tests
{
	public class TarjetaTests
	{
		private readonly DateTime martesALas10 = new DateTime(2026, 11, 3, 10, 0, 0);

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

		public void RecargarSaldo_MontosValidos_SumaElSaldoYDevuelveTrue(decimal montoRecarga)
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

			Assert.That(boletoGenerado.TarifaBasica, Is.EqualTo(2150));

			Assert.That(boletoGenerado.TarjetaId, Is.EqualTo(tarjeta.Id));

			Assert.That(tarjeta.Saldo, Is.EqualTo(3000 - 2150));
		}

		[Test]
		public void PagarCon_SuperaElSaldoNegativoPermitido_DevuelveFalseYNoGeneraBoleto()
		{

			var tarjeta = new Tarjeta();
			tarjeta.Id = 2;
			tarjeta.NumeroTarjeta = "6061267008306632";
			tarjeta.RecargarSaldo(2000);

			var colectivo = new Colectivo();

			bool viajePlus = colectivo.PagarCon(tarjeta);

			Assert.That(viajePlus, Is.True);
			Assert.That(tarjeta.Saldo, Is.EqualTo(2000 - 2150));

			bool pago = colectivo.PagarCon(tarjeta);

			Assert.That(pago, Is.False);

			Assert.That(tarjeta.Boletos, Has.Count.EqualTo(1));

			Assert.That(tarjeta.Saldo, Is.EqualTo(2000 - 2150));

		}

		[Test]
		public void PagarCon_VariosViajes_LaTarjetaAcumulaUnBoletoPorViaje()
		{
			var tarjeta = new Tarjeta { Id = 3, NumeroTarjeta = "6061267008306633" };
			tarjeta.RecargarSaldo(8000);

			var colectivo = new Colectivo();

			Assert.That(colectivo.PagarCon(tarjeta), Is.True);
			Assert.That(colectivo.PagarCon(tarjeta), Is.True);
			Assert.That(colectivo.PagarCon(tarjeta), Is.True);
			Assert.That(colectivo.PagarCon(tarjeta), Is.True);
			Assert.That(colectivo.PagarCon(tarjeta), Is.False);

			Assert.That(tarjeta.Boletos, Has.Count.EqualTo(4));
			Assert.That(tarjeta.Saldo, Is.EqualTo(8000 - 4 * 2150));
		}

		[Test]
		public void RecargarSaldo_ConSaldoNegativo_DescuentaLoAdeudado()
		{
			var tarjeta = new Tarjeta();
			tarjeta.RecargarSaldo(2000);
			new Colectivo().PagarCon(tarjeta);

			bool resultado = tarjeta.RecargarSaldo(3000);

			Assert.That(resultado, Is.True);
			Assert.That(tarjeta.Saldo, Is.EqualTo(3000 - 150));
		}

		[Test]
		public void PuedePagar_SaldoCero_DevuelveTrue()
		{
			var tarjeta = new Tarjeta();

			Assert.That(tarjeta.PuedePagar(2000), Is.True);
		}

		[Test]
		public void PuedePagar_QuedariaPorDebajoDelSaldoNegativoPermitido_DevuelveFalse()
		{
			var tarjeta = new Tarjeta();
			tarjeta.DescontarSaldo(2000);

			Assert.That(tarjeta.PuedePagar(2150), Is.False);
		}

		[Test]
		public void DescontarSaldo_SaldoCero_QuedaEnNegativo()
		{
			var tarjeta = new Tarjeta();

			tarjeta.DescontarSaldo(1000);

			Assert.That(tarjeta.Saldo, Is.EqualTo(-1000));
		}

		[Test]
		public void DescontarSaldo_QuedariaPorDebajoDelSaldoNegativoPermitido_NoDescuenta()
		{
			var tarjeta = new Tarjeta();
			tarjeta.DescontarSaldo(1500);

			tarjeta.DescontarSaldo(1000);

			Assert.That(tarjeta.Saldo, Is.EqualTo(-1500));
		}

		[Test]
		public void PagarCon_FranquiciaCompleta_SiemprePuedePagarInclusoConSaldoCero()
		{
			var tarjeta = new Tarjeta
			{
				Id = 10,
				NumeroTarjeta = "10101010",
				TarjetaTipo = new FranquiciaCompleta()
			};

			var colectivo = new Colectivo();
			bool pago = colectivo.PagarCon(tarjeta, martesALas10);

			Assert.That(pago, Is.True);
			Assert.That(tarjeta.Boletos, Has.Count.EqualTo(1));
			Assert.That(tarjeta.Boletos[0].TotalAbonado, Is.EqualTo(0));
			Assert.That(tarjeta.Saldo, Is.EqualTo(0));
		}

		[Test]
		public void PagarCon_MedioBoletoEstudiantil_MontoDelBoletoEsLaMitadDelNormal()
		{
			var tarjetaNormal = new Tarjeta
			{
				Id = 20,
				NumeroTarjeta = "20202020",
				TarjetaTipo = new Normal()
			};
			tarjetaNormal.RecargarSaldo(3000);

			var tarjetaMedio = new Tarjeta
			{
				Id = 21,
				NumeroTarjeta = "21212121",
				TarjetaTipo = new MedioBoleto()
			};
			tarjetaMedio.RecargarSaldo(2000);

			var colectivo = new Colectivo();

			Assert.That(colectivo.PagarCon(tarjetaNormal, martesALas10), Is.True);
			Assert.That(colectivo.PagarCon(tarjetaMedio, martesALas10), Is.True);

			Boleto boletoNormal = tarjetaNormal.Boletos[0];
			Boleto boletoMedio = tarjetaMedio.Boletos[0];

			Assert.That(boletoMedio.TotalAbonado, Is.EqualTo(boletoNormal.TotalAbonado / 2));
			Assert.That(boletoMedio.TotalAbonado, Is.EqualTo(1075));
			Assert.That(tarjetaMedio.Saldo, Is.EqualTo(2000 - 1075));
		}

		[Test]
		public void PagarCon_BoletoGratuitoEstudiantil_GeneraBoletoConMontoCero()
		{
			var tarjeta = new Tarjeta
			{
				Id = 30,
				NumeroTarjeta = "30303030",
				TarjetaTipo = new BoletoGratuito()
			};

			var colectivo = new Colectivo();
			bool pago = colectivo.PagarCon(tarjeta, martesALas10);

			Assert.That(pago, Is.True);
			Assert.That(tarjeta.Boletos, Has.Count.EqualTo(1));
			Assert.That(tarjeta.Boletos[0].TotalAbonado, Is.EqualTo(0));
		}

		[Test]
		public void TarjetaTipo_NuevoTipoSinCrearClase_CalculaTarifaCorrectamente()
		{
			var tipoUniversitario = new MedioBoleto { Nombre = "Medio boleto universitario" };
			var tarjeta = new Tarjeta { TarjetaTipo = tipoUniversitario };

			decimal tarifa = tarjeta.CalcularTarifa(2150);

			Assert.That(tarifa, Is.EqualTo(1075));
		}

		[Test]
		public void PagarCon_MedioBoleto_NoPermiteViajarAntesDePasarCincoMinutos()
		{
			var tarjeta = new Tarjeta
			{
				Id = 40,
				NumeroTarjeta = "40404040",
				TarjetaTipo = new MedioBoleto()
			};
			tarjeta.RecargarSaldo(3000);

			var colectivo = new Colectivo();
			DateTime fechaInicial = new DateTime(2026, 10, 22, 10, 0, 0);

			bool primerViaje = colectivo.PagarCon(tarjeta, fechaInicial);
			Assert.That(primerViaje, Is.True);

			bool viajeAntesDeCincoMinutos = colectivo.PagarCon(tarjeta, fechaInicial.AddMinutes(3));
			Assert.That(viajeAntesDeCincoMinutos, Is.False);
			Assert.That(tarjeta.Boletos, Has.Count.EqualTo(1));

			bool viajePasadosCincoMinutos = colectivo.PagarCon(tarjeta, fechaInicial.AddMinutes(6));
			Assert.That(viajePasadosCincoMinutos, Is.True);
			Assert.That(tarjeta.Boletos, Has.Count.EqualTo(2));
		}

		[Test]
		public void PagarCon_MedioBoleto_NoPermiteMasDeDosMedioBoletosPorDiaYTercerViajeCobraNormal()
		{
			var tarjeta = new Tarjeta
			{
				Id = 41,
				NumeroTarjeta = "41414141",
				TarjetaTipo = new MedioBoleto()
			};
			tarjeta.RecargarSaldo(5000);

			var colectivo = new Colectivo();
			DateTime fechaHoy = new DateTime(2026, 10, 22, 8, 0, 0);

			Assert.That(colectivo.PagarCon(tarjeta, fechaHoy), Is.True);
			Assert.That(tarjeta.Boletos[0].TotalAbonado, Is.EqualTo(1075));

			Assert.That(colectivo.PagarCon(tarjeta, fechaHoy.AddMinutes(10)), Is.True);
			Assert.That(tarjeta.Boletos[1].TotalAbonado, Is.EqualTo(1075));

			Assert.That(colectivo.PagarCon(tarjeta, fechaHoy.AddMinutes(20)), Is.True);
			Assert.That(tarjeta.Boletos[2].TotalAbonado, Is.EqualTo(2150));

			DateTime diaSiguiente = fechaHoy.AddDays(1);
			Assert.That(colectivo.PagarCon(tarjeta, diaSiguiente), Is.True);
			Assert.That(tarjeta.Boletos[3].TotalAbonado, Is.EqualTo(1075));
		}

		[Test]
		public void RecargarSaldo_SuperaElMaximo_AcreditaHastaElMaximoYDejaElExcedentePendiente()
		{
			var tarjeta = new Tarjeta();

			Assert.That(tarjeta.RecargarSaldo(30000), Is.True);
			Assert.That(tarjeta.RecargarSaldo(30000), Is.True);

			Assert.That(tarjeta.Saldo, Is.EqualTo(Tarjeta.SaldoMaximo));
			Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(60000 - 56000));
		}

		[Test]
		public void PagarCon_ConSaldoPendiente_AcreditaElPendienteHastaElMaximo()
		{
			var tarjeta = new Tarjeta();
			tarjeta.RecargarSaldo(30000);
			tarjeta.RecargarSaldo(30000);

			bool pago = new Colectivo().PagarCon(tarjeta);

			Assert.That(pago, Is.True);
			Assert.That(tarjeta.Saldo, Is.EqualTo(Tarjeta.SaldoMaximo));
			Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(4000 - 2150));
		}

	}
}
