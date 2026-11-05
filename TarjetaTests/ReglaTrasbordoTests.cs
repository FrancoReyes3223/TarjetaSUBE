using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	public class ReglaTrasbordoTests
	{
		private readonly DateTime lunesALas10 = new DateTime(2026, 11, 2, 10, 0, 0);
		private readonly ReglaTrasbordo regla = new ReglaTrasbordo();

		private static HistorialViajes Historial(params Boleto[] boletos)
		{
			return new HistorialViajes(new List<Boleto>(boletos));
		}

		[Test]
		public void EsTrasbordo_SinViajesPrevios_DevuelveFalse()
		{
			Assert.That(regla.EsTrasbordo(Historial(), "K", lunesALas10), Is.False);
		}

		[Test]
		public void EsTrasbordo_LineaVacia_DevuelveFalse()
		{
			var historial = Historial(new Boleto { LineaColectivo = "K", FechayHora = lunesALas10 });

			Assert.That(regla.EsTrasbordo(historial, string.Empty, lunesALas10.AddMinutes(10)), Is.False);
		}

		[Test]
		public void EsTrasbordo_UltimoBoletoSinLinea_DevuelveFalse()
		{
			var historial = Historial(new Boleto { FechayHora = lunesALas10 });

			Assert.That(regla.EsTrasbordo(historial, "K", lunesALas10.AddMinutes(10)), Is.False);
		}

		[TestCase(0, true)]
		[TestCase(60, true)]
		[TestCase(61, false)]
		public void EsTrasbordo_SegunMinutosDesdeElPrimerViaje(int minutos, bool esperado)
		{
			var historial = Historial(new Boleto { LineaColectivo = "K", FechayHora = lunesALas10 });

			Assert.That(regla.EsTrasbordo(historial, "102", lunesALas10.AddMinutes(minutos)), Is.EqualTo(esperado));
		}

		[Test]
		public void EsTrasbordo_CuentaLaHoraDesdeElUltimoViajePagado()
		{
			var historial = Historial(
				new Boleto { LineaColectivo = "K", FechayHora = lunesALas10 },
				new Boleto { LineaColectivo = "102", FechayHora = lunesALas10.AddMinutes(50), EsTrasbordo = true });

			Assert.That(regla.EsTrasbordo(historial, "133", lunesALas10.AddMinutes(59)), Is.True);
			Assert.That(regla.EsTrasbordo(historial, "133", lunesALas10.AddMinutes(70)), Is.False);
		}

		[Test]
		public void EsTrasbordo_FechaAnteriorAlPrimerViaje_DevuelveFalse()
		{
			var historial = Historial(new Boleto { LineaColectivo = "K", FechayHora = lunesALas10 });

			Assert.That(regla.EsTrasbordo(historial, "102", lunesALas10.AddMinutes(-5)), Is.False);
		}

		[TestCase(2026, 11, 7, 7, 0, true)]
		[TestCase(2026, 11, 7, 21, 59, true)]
		[TestCase(2026, 11, 2, 6, 59, false)]
		[TestCase(2026, 11, 2, 22, 0, false)]
		[TestCase(2026, 11, 8, 12, 0, false)]
		public void EsTrasbordo_SegunDiaYFranjaHoraria(int anio, int mes, int dia, int hora, int minuto, bool esperado)
		{
			DateTime fechaHora = new DateTime(anio, mes, dia, hora, minuto, 0);
			var historial = Historial(new Boleto { LineaColectivo = "K", FechayHora = fechaHora.AddMinutes(-10) });

			Assert.That(regla.EsTrasbordo(historial, "102", fechaHora), Is.EqualTo(esperado));
		}
	}
}
