using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	public class HistorialViajesTests
	{
		private readonly DateTime lunesALas10 = new DateTime(2026, 11, 2, 10, 0, 0);

		private static HistorialViajes Historial(params Boleto[] boletos)
		{
			return new HistorialViajes(new List<Boleto>(boletos));
		}

		[Test]
		public void UltimoBoleto_SinViajes_DevuelveNull()
		{
			Assert.That(Historial().UltimoBoleto(), Is.Null);
			Assert.That(Historial().UltimoViajePagado(), Is.Null);
		}

		[Test]
		public void UltimoBoleto_DevuelveElMasReciente()
		{
			var ultimo = new Boleto { LineaColectivo = "102", FechayHora = lunesALas10.AddMinutes(20), EsTrasbordo = true };
			var historial = Historial(new Boleto { LineaColectivo = "K", FechayHora = lunesALas10 }, ultimo);

			Assert.That(historial.UltimoBoleto(), Is.SameAs(ultimo));
		}

		[Test]
		public void UltimoViajePagado_IgnoraLosTrasbordos()
		{
			var pagado = new Boleto { LineaColectivo = "K", FechayHora = lunesALas10 };
			var historial = Historial(pagado, new Boleto { LineaColectivo = "102", FechayHora = lunesALas10.AddMinutes(20), EsTrasbordo = true });

			Assert.That(historial.UltimoViajePagado(), Is.SameAs(pagado));
		}

		[Test]
		public void ViajesDelDia_NoCuentaTrasbordosNiOtrosDias()
		{
			var historial = Historial(
				new Boleto { FechayHora = lunesALas10.AddDays(-1) },
				new Boleto { FechayHora = lunesALas10 },
				new Boleto { FechayHora = lunesALas10.AddMinutes(20), EsTrasbordo = true },
				new Boleto { FechayHora = lunesALas10.AddHours(3) });

			Assert.That(historial.ViajesDelDia(lunesALas10), Is.EqualTo(2));
		}

		[Test]
		public void ViajesDelMes_NoCuentaTrasbordosNiOtrosMeses()
		{
			var historial = Historial(
				new Boleto { FechayHora = lunesALas10.AddMonths(-1) },
				new Boleto { FechayHora = lunesALas10 },
				new Boleto { FechayHora = lunesALas10.AddMinutes(20), EsTrasbordo = true },
				new Boleto { FechayHora = lunesALas10.AddDays(5) });

			Assert.That(historial.ViajesDelMes(lunesALas10), Is.EqualTo(2));
		}

		[Test]
		public void PasaronMinutosDesdeUltimoViaje_SinViajes_DevuelveTrue()
		{
			Assert.That(Historial().PasaronMinutosDesdeUltimoViaje(lunesALas10, 5), Is.True);
		}

		[TestCase(4, false)]
		[TestCase(5, true)]
		public void PasaronMinutosDesdeUltimoViaje_SegunMinutos(int minutos, bool esperado)
		{
			var historial = Historial(new Boleto { FechayHora = lunesALas10 });

			Assert.That(historial.PasaronMinutosDesdeUltimoViaje(lunesALas10.AddMinutes(minutos), 5), Is.EqualTo(esperado));
		}
	}
}
