using System;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	public class UsoFrecuenteTests
	{
		private readonly DateTime inicioDeOctubre = new DateTime(2026, 10, 1, 8, 0, 0);

		private static void AgregarViajes(Tarjeta tarjeta, int cantidad, DateTime desde)
		{
			for (int i = 0; i < cantidad; i++)
			{
				tarjeta.Boletos.Add(new Boleto { FechayHora = desde.AddMinutes(i * 10) });
			}
		}

		[TestCase(0, 1580)]
		[TestCase(28, 1580)]
		[TestCase(29, 1264)]
		[TestCase(78, 1264)]
		[TestCase(79, 1185)]
		[TestCase(120, 1185)]
		public void CalcularTarifa_TarjetaNormal_AplicaDescuentoSegunViajesDelMes(int viajesPrevios, int tarifaEsperada)
		{
			var tarjeta = new Tarjeta { TarjetaTipo = new TarjetaTipo("Normal", 100) };
			AgregarViajes(tarjeta, viajesPrevios, inicioDeOctubre);

			int tarifa = tarjeta.CalcularTarifa(Colectivo.TarifaBasica, new DateTime(2026, 10, 31, 20, 0, 0));

			Assert.That(tarifa, Is.EqualTo(tarifaEsperada));
		}

		[Test]
		public void CalcularTarifa_TarjetaSinTipo_AplicaUsoFrecuente()
		{
			var tarjeta = new Tarjeta();
			AgregarViajes(tarjeta, 29, inicioDeOctubre);

			int tarifa = tarjeta.CalcularTarifa(Colectivo.TarifaBasica, new DateTime(2026, 10, 31, 20, 0, 0));

			Assert.That(tarifa, Is.EqualTo(1264));
		}

		[Test]
		public void CalcularTarifa_ViajesDelMesAnterior_NoCuentan()
		{
			var tarjeta = new Tarjeta();
			AgregarViajes(tarjeta, 85, new DateTime(2026, 9, 1, 8, 0, 0));

			int tarifa = tarjeta.CalcularTarifa(Colectivo.TarifaBasica, new DateTime(2026, 10, 1, 8, 0, 0));

			Assert.That(tarjeta.ViajesDelMes(new DateTime(2026, 10, 1)), Is.EqualTo(0));
			Assert.That(tarifa, Is.EqualTo(1580));
		}

		[Test]
		public void CalcularTarifa_MismoMesDeOtroAnio_NoCuenta()
		{
			var tarjeta = new Tarjeta();
			AgregarViajes(tarjeta, 85, new DateTime(2025, 10, 1, 8, 0, 0));

			int tarifa = tarjeta.CalcularTarifa(Colectivo.TarifaBasica, inicioDeOctubre);

			Assert.That(tarifa, Is.EqualTo(1580));
		}

		[Test]
		public void CalcularTarifa_PrimerDiaDelMes_ReiniciaElConteo()
		{
			var tarjeta = new Tarjeta();
			AgregarViajes(tarjeta, 85, new DateTime(2026, 10, 1, 8, 0, 0));

			Assert.That(tarjeta.CalcularTarifa(Colectivo.TarifaBasica, new DateTime(2026, 10, 31, 23, 59, 0)), Is.EqualTo(1185));
			Assert.That(tarjeta.CalcularTarifa(Colectivo.TarifaBasica, new DateTime(2026, 11, 1, 0, 0, 0)), Is.EqualTo(1580));
		}

		[Test]
		public void CalcularTarifa_MedioBoleto_NoAplicaUsoFrecuente()
		{
			var tarjeta = new Tarjeta { TarjetaTipo = new TarjetaTipo("Medio boleto estudiantil", 50) };
			AgregarViajes(tarjeta, 40, inicioDeOctubre);

			int tarifa = tarjeta.CalcularTarifa(Colectivo.TarifaBasica, new DateTime(2026, 10, 31, 10, 0, 0));

			Assert.That(tarifa, Is.EqualTo(790));
		}

		[TestCase("Boleto gratuito estudiantil")]
		[TestCase("Franquicia completa")]
		public void CalcularTarifa_FranquiciaGratuita_NoAplicaUsoFrecuente(string nombreTipo)
		{
			var tarjeta = new Tarjeta { TarjetaTipo = new TarjetaTipo(nombreTipo, 0) };
			AgregarViajes(tarjeta, 85, inicioDeOctubre);

			int tarifa = tarjeta.CalcularTarifa(Colectivo.TarifaBasica, new DateTime(2026, 10, 31, 10, 0, 0));

			Assert.That(tarifa, Is.EqualTo(0));
		}

		[Test]
		public void EsNormal_SegunTipoDeTarjeta()
		{
			Assert.That(new Tarjeta().EsNormal(), Is.True);
			Assert.That(new Tarjeta { TarjetaTipo = new TarjetaTipo("Normal", 100) }.EsNormal(), Is.True);
			Assert.That(new Tarjeta { TarjetaTipo = new TarjetaTipo("Medio boleto estudiantil", 50) }.EsNormal(), Is.False);
			Assert.That(new Tarjeta { TarjetaTipo = new TarjetaTipo("Franquicia completa", 0) }.EsNormal(), Is.False);
		}

		[Test]
		public void PagarCon_TarjetaNormal_Viaje30GeneraBoletoConDescuento()
		{
			var tarjeta = new Tarjeta { Id = 60 };
			AgregarViajes(tarjeta, 29, inicioDeOctubre);
			tarjeta.RecargarSaldo(5000);

			bool pago = new Colectivo { Linea = "K" }.PagarCon(tarjeta, new DateTime(2026, 10, 31, 20, 0, 0));

			Assert.That(pago, Is.True);
			Boleto boleto = tarjeta.Boletos[29];
			Assert.That(boleto.TarifaBasica, Is.EqualTo(1580));
			Assert.That(boleto.TotalAbonado, Is.EqualTo(1264));
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - 1264));
		}

		[Test]
		public void PagarCon_TarjetaNormal_ViajesConsecutivosCambianDeTramo()
		{
			var tarjeta = new Tarjeta();
			AgregarViajes(tarjeta, 28, inicioDeOctubre);
			tarjeta.RecargarSaldo(10000);

			var colectivo = new Colectivo { Linea = "K" };
			DateTime noche = new DateTime(2026, 10, 31, 20, 0, 0);

			colectivo.PagarCon(tarjeta, noche);
			colectivo.PagarCon(tarjeta, noche.AddMinutes(30));

			Assert.That(tarjeta.Boletos[28].TotalAbonado, Is.EqualTo(1580));
			Assert.That(tarjeta.Boletos[29].TotalAbonado, Is.EqualTo(1264));
			Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 1580 - 1264));
		}
	}
}
