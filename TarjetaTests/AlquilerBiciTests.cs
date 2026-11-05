using System;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	public class AlquilerBiciTests
	{
		private readonly DateTime retiro = new DateTime(2026, 11, 4, 9, 0, 0);

		[Test]
		public void EstaEnCurso_SinDevolucion_DevuelveTrue()
		{
			var alquiler = new AlquilerBici { FechaRetiro = retiro };

			Assert.That(alquiler.EstaEnCurso(), Is.True);
		}

		[Test]
		public void EstaEnCurso_ConDevolucion_DevuelveFalse()
		{
			var alquiler = new AlquilerBici { FechaRetiro = retiro, FechaDevolucion = retiro.AddMinutes(20) };

			Assert.That(alquiler.EstaEnCurso(), Is.False);
		}

		[Test]
		public void ExcedioTiempoMaximo_EnCurso_DevuelveFalse()
		{
			var alquiler = new AlquilerBici { FechaRetiro = retiro };

			Assert.That(alquiler.ExcedioTiempoMaximo(), Is.False);
		}

		[TestCase(60, false)]
		[TestCase(61, true)]
		public void ExcedioTiempoMaximo_SegunMinutosDeUso(int minutos, bool esperado)
		{
			var alquiler = new AlquilerBici { FechaRetiro = retiro, FechaDevolucion = retiro.AddMinutes(minutos) };

			Assert.That(alquiler.ExcedioTiempoMaximo(), Is.EqualTo(esperado));
		}

		[Test]
		public void TieneMultaPendiente_ExcedidoSinCobrar_DevuelveTrue()
		{
			var alquiler = new AlquilerBici { FechaRetiro = retiro, FechaDevolucion = retiro.AddMinutes(90) };

			Assert.That(alquiler.TieneMultaPendiente(), Is.True);
		}

		[Test]
		public void TieneMultaPendiente_MultaYaCobrada_DevuelveFalse()
		{
			var alquiler = new AlquilerBici
			{
				FechaRetiro = retiro,
				FechaDevolucion = retiro.AddMinutes(90),
				MultaCobrada = true
			};

			Assert.That(alquiler.TieneMultaPendiente(), Is.False);
		}

		[Test]
		public void TieneMultaPendiente_DevueltaATiempo_DevuelveFalse()
		{
			var alquiler = new AlquilerBici { FechaRetiro = retiro, FechaDevolucion = retiro.AddMinutes(45) };

			Assert.That(alquiler.TieneMultaPendiente(), Is.False);
		}
	}
}
