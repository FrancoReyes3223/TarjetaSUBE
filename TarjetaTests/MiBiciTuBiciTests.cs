using System;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	public class MiBiciTuBiciTests
	{
		private readonly DateTime manana = new DateTime(2026, 11, 4, 9, 0, 0);

		private static Tarjeta CrearTarjetaConSaldo(params decimal[] recargas)
		{
			var tarjeta = new Tarjeta { Id = 1 };
			foreach (decimal recarga in recargas)
			{
				tarjeta.RecargarSaldo(recarga);
			}
			return tarjeta;
		}

		private static void GenerarMulta(EstacionBicicletas estacion, Tarjeta tarjeta, DateTime retiro)
		{
			Assert.That(estacion.RetirarBici(tarjeta, retiro), Is.True);
			Assert.That(estacion.DevolverBici(tarjeta, retiro.AddMinutes(AlquilerBici.MinutosMaximosDeUso + 1)), Is.True);
		}

		[Test]
		public void RetirarBici_ConSaldoSuficienteYSinMultas_CobraTarifaDiaria()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(5000m);

			bool retirada = estacion.RetirarBici(tarjeta, manana);

			Assert.That(retirada, Is.True);
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000m - EstacionBicicletas.TarifaDiaria));
			Assert.That(estacion.Alquileres, Has.Count.EqualTo(1));
			Assert.That(estacion.Alquileres[0].MontoAbonado, Is.EqualTo(2418.75m));
			Assert.That(estacion.Alquileres[0].EstaEnCurso(), Is.True);
		}

		[Test]
		public void RetirarBici_DespuesDeDevolverATiempo_NoCobraMulta()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(10000m);

			estacion.RetirarBici(tarjeta, manana);
			estacion.DevolverBici(tarjeta, manana.AddMinutes(AlquilerBici.MinutosMaximosDeUso));

			Assert.That(estacion.Alquileres[0].ExcedioTiempoMaximo(), Is.False);
			Assert.That(estacion.MultasDelDia(tarjeta, manana), Is.EqualTo(0));
			Assert.That(estacion.CalcularCostoRetiro(tarjeta, manana.AddHours(2)), Is.EqualTo(EstacionBicicletas.TarifaDiaria));

			bool retirada = estacion.RetirarBici(tarjeta, manana.AddHours(2));

			Assert.That(retirada, Is.True);
			Assert.That(tarjeta.Saldo, Is.EqualTo(10000m - 2 * EstacionBicicletas.TarifaDiaria));
		}

		[Test]
		public void RetirarBici_SaldoQueSoloAlcanzaConSaldoNegativo_PermiteRetirar()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(2000m);

			bool retirada = estacion.RetirarBici(tarjeta, manana);

			Assert.That(retirada, Is.True);
			Assert.That(tarjeta.Saldo, Is.EqualTo(-418.75m));
		}

		[Test]
		public void RetirarBici_SinSaldoSuficienteYSinMultas_NoPermiteRetirar()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo();

			bool retirada = estacion.RetirarBici(tarjeta, manana);

			Assert.That(retirada, Is.False);
			Assert.That(tarjeta.Saldo, Is.EqualTo(0m));
			Assert.That(estacion.Alquileres, Is.Empty);
		}

		[Test]
		public void RetirarBici_SinSaldoSuficienteConMultaPendiente_NoPermiteRetirar()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(2000m);
			GenerarMulta(estacion, tarjeta, manana);
			decimal saldoAntes = tarjeta.Saldo;

			bool retirada = estacion.RetirarBici(tarjeta, manana.AddHours(3));

			Assert.That(saldoAntes, Is.EqualTo(-418.75m));
			Assert.That(retirada, Is.False);
			Assert.That(tarjeta.Saldo, Is.EqualTo(saldoAntes));
			Assert.That(estacion.Alquileres, Has.Count.EqualTo(1));
		}

		[Test]
		public void RetirarBici_SaldoAlcanzaParaTarifaPeroNoParaLaMulta_NoPermiteRetirar()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(3000m);
			GenerarMulta(estacion, tarjeta, manana);

			Assert.That(tarjeta.PuedePagar(EstacionBicicletas.TarifaDiaria), Is.True);
			Assert.That(estacion.CalcularCostoRetiro(tarjeta, manana.AddHours(3)), Is.EqualTo(3668.75m));

			bool retirada = estacion.RetirarBici(tarjeta, manana.AddHours(3));

			Assert.That(retirada, Is.False);
			Assert.That(tarjeta.Saldo, Is.EqualTo(581.25m));
		}

		[Test]
		public void RetirarBici_ConBiciEnUso_NoPermiteRetirarOtra()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(10000m);
			estacion.RetirarBici(tarjeta, manana);

			bool retirada = estacion.RetirarBici(tarjeta, manana.AddMinutes(10));

			Assert.That(retirada, Is.False);
			Assert.That(tarjeta.Saldo, Is.EqualTo(10000m - EstacionBicicletas.TarifaDiaria));
		}

		[Test]
		public void RetirarBici_ConUnaMultaAcumulada_CobraTarifaMasMulta()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(10000m);
			GenerarMulta(estacion, tarjeta, manana);
			decimal saldoAntes = tarjeta.Saldo;

			bool retirada = estacion.RetirarBici(tarjeta, manana.AddHours(3));

			Assert.That(retirada, Is.True);
			Assert.That(estacion.MultasDelDia(tarjeta, manana.AddHours(3)), Is.EqualTo(1));
			Assert.That(tarjeta.Saldo, Is.EqualTo(saldoAntes - 3668.75m));
			Assert.That(estacion.Alquileres[1].MontoAbonado, Is.EqualTo(EstacionBicicletas.TarifaDiaria + EstacionBicicletas.ValorMulta));
		}

		[Test]
		public void RetirarBici_ConVariasMultasAcumuladas_CobraTarifaMasTodasLasMultas()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(30000m);
			GenerarMulta(estacion, tarjeta, manana);
			GenerarMulta(estacion, tarjeta, manana.AddHours(2));
			GenerarMulta(estacion, tarjeta, manana.AddHours(4));
			decimal saldoAntes = tarjeta.Saldo;

			bool retirada = estacion.RetirarBici(tarjeta, manana.AddHours(6));

			Assert.That(retirada, Is.True);
			Assert.That(estacion.MultasDelDia(tarjeta, manana.AddHours(6)), Is.EqualTo(3));
			Assert.That(estacion.Alquileres[3].MontoAbonado, Is.EqualTo(2418.75m + 3 * 1250m));
			Assert.That(tarjeta.Saldo, Is.EqualTo(saldoAntes - 6168.75m));
		}

		[Test]
		public void RetirarBici_MultasDeOtroDia_NoSeCobran()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(10000m);
			GenerarMulta(estacion, tarjeta, manana);
			decimal saldoAntes = tarjeta.Saldo;

			bool retirada = estacion.RetirarBici(tarjeta, manana.AddDays(1));

			Assert.That(retirada, Is.True);
			Assert.That(tarjeta.Saldo, Is.EqualTo(saldoAntes - EstacionBicicletas.TarifaDiaria));
		}

		[Test]
		public void MultasDelDia_SoloCuentaLasDeLaMismaTarjeta()
		{
			var estacion = new EstacionBicicletas();
			var tarjetaConMulta = CrearTarjetaConSaldo(10000m);
			var otraTarjeta = new Tarjeta { Id = 2 };
			otraTarjeta.RecargarSaldo(10000m);
			GenerarMulta(estacion, tarjetaConMulta, manana);

			Assert.That(estacion.MultasDelDia(otraTarjeta, manana), Is.EqualTo(0));
			Assert.That(estacion.CalcularCostoRetiro(otraTarjeta, manana.AddHours(3)), Is.EqualTo(EstacionBicicletas.TarifaDiaria));
		}
	}
}
