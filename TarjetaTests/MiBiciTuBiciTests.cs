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

		private static void UsarBiciConDemora(EstacionBicicletas estacion, Tarjeta tarjeta, DateTime retiro)
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
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000m - 2418.75m));
			Assert.That(estacion.Alquileres, Has.Count.EqualTo(1));
			Assert.That(estacion.Alquileres[0].MontoAbonado, Is.EqualTo(2418.75m));
			Assert.That(estacion.Alquileres[0].EstaEnCurso(), Is.True);
		}

		[Test]
		public void RetirarBici_SinFecha_UsaLaFechaActual()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(5000m);

			DateTime antes = DateTime.Now;
			bool retirada = estacion.RetirarBici(tarjeta);
			DateTime despues = DateTime.Now;

			Assert.That(retirada, Is.True);
			Assert.That(estacion.Alquileres[0].FechaRetiro, Is.InRange(antes, despues));
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000m - EstacionBicicletas.TarifaDiaria));
		}

		[Test]
		public void RetirarBici_DevolucionATiempoYOtroRetiroElMismoDia_NoCobraNuevamente()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(10000m);

			estacion.RetirarBici(tarjeta, manana);
			estacion.DevolverBici(tarjeta, manana.AddMinutes(AlquilerBici.MinutosMaximosDeUso));

			Assert.That(estacion.MultasPendientes(tarjeta), Is.EqualTo(0));
			Assert.That(estacion.PagoTarifaDelDia(tarjeta, manana.AddHours(2)), Is.True);
			Assert.That(estacion.CalcularCostoRetiro(tarjeta, manana.AddHours(2)), Is.EqualTo(0m));

			bool retirada = estacion.RetirarBici(tarjeta, manana.AddHours(2));

			Assert.That(retirada, Is.True);
			Assert.That(estacion.Alquileres[1].MontoAbonado, Is.EqualTo(0m));
			Assert.That(tarjeta.Saldo, Is.EqualTo(10000m - EstacionBicicletas.TarifaDiaria));
		}

		[Test]
		public void RetirarBici_AlDiaSiguiente_CobraNuevamenteLaTarifaDiaria()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(10000m);
			estacion.RetirarBici(tarjeta, manana);
			estacion.DevolverBici(tarjeta, manana.AddMinutes(30));

			Assert.That(estacion.PagoTarifaDelDia(tarjeta, manana.AddDays(1)), Is.False);

			bool retirada = estacion.RetirarBici(tarjeta, manana.AddDays(1));

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
			UsarBiciConDemora(estacion, tarjeta, manana);
			decimal saldoAntes = tarjeta.Saldo;

			bool retirada = estacion.RetirarBici(tarjeta, manana.AddDays(1));

			Assert.That(saldoAntes, Is.EqualTo(-418.75m));
			Assert.That(retirada, Is.False);
			Assert.That(tarjeta.Saldo, Is.EqualTo(saldoAntes));
			Assert.That(estacion.Alquileres, Has.Count.EqualTo(1));
			Assert.That(estacion.MultasPendientes(tarjeta), Is.EqualTo(1));
		}

		[Test]
		public void RetirarBici_SaldoAlcanzaParaTarifaPeroNoParaLaMulta_NoPermiteRetirar()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(3000m);
			UsarBiciConDemora(estacion, tarjeta, manana);
			DateTime diaSiguiente = manana.AddDays(1);

			Assert.That(tarjeta.PuedePagar(EstacionBicicletas.TarifaDiaria), Is.True);
			Assert.That(estacion.CalcularCostoRetiro(tarjeta, diaSiguiente), Is.EqualTo(3668.75m));

			bool retirada = estacion.RetirarBici(tarjeta, diaSiguiente);

			Assert.That(retirada, Is.False);
			Assert.That(tarjeta.Saldo, Is.EqualTo(581.25m));
			Assert.That(estacion.MultasPendientes(tarjeta), Is.EqualTo(1));
		}

		[Test]
		public void RetirarBici_ConBiciEnUso_NoPermiteRetirarOtra()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(10000m);
			estacion.RetirarBici(tarjeta, manana);

			bool retirada = estacion.RetirarBici(tarjeta, manana.AddMinutes(10));

			Assert.That(retirada, Is.False);
			Assert.That(estacion.Alquileres, Has.Count.EqualTo(1));
			Assert.That(tarjeta.Saldo, Is.EqualTo(10000m - EstacionBicicletas.TarifaDiaria));
		}

		[Test]
		public void RetirarBici_ConUnaMultaAcumulada_CobraTarifaMasMulta()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(10000m);
			UsarBiciConDemora(estacion, tarjeta, manana);
			decimal saldoAntes = tarjeta.Saldo;
			DateTime diaSiguiente = manana.AddDays(1);

			Assert.That(estacion.MultasPendientes(tarjeta), Is.EqualTo(1));

			bool retirada = estacion.RetirarBici(tarjeta, diaSiguiente);

			Assert.That(retirada, Is.True);
			Assert.That(tarjeta.Saldo, Is.EqualTo(saldoAntes - 3668.75m));
			Assert.That(estacion.Alquileres[1].MontoAbonado, Is.EqualTo(EstacionBicicletas.TarifaDiaria + EstacionBicicletas.ValorMulta));
			Assert.That(estacion.MultasPendientes(tarjeta), Is.EqualTo(0));
		}

		[Test]
		public void RetirarBici_ConVariasMultasDelMismoDia_SeAcumulanYSeCobranJuntas()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(30000m);
			UsarBiciConDemora(estacion, tarjeta, manana);
			UsarBiciConDemora(estacion, tarjeta, manana.AddHours(2));
			UsarBiciConDemora(estacion, tarjeta, manana.AddHours(4));

			Assert.That(estacion.MultasPendientes(tarjeta), Is.EqualTo(3));
			Assert.That(tarjeta.Saldo, Is.EqualTo(30000m - EstacionBicicletas.TarifaDiaria));

			bool retirada = estacion.RetirarBici(tarjeta, manana.AddDays(1));

			Assert.That(retirada, Is.True);
			Assert.That(estacion.Alquileres[3].MontoAbonado, Is.EqualTo(2418.75m + 3 * 1250m));
			Assert.That(tarjeta.Saldo, Is.EqualTo(30000m - 2418.75m - 6168.75m));
			Assert.That(estacion.MultasPendientes(tarjeta), Is.EqualTo(0));
		}

		[Test]
		public void RetirarBici_MultasYaCobradas_NoSeVuelvenACobrar()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(30000m);
			UsarBiciConDemora(estacion, tarjeta, manana);

			estacion.RetirarBici(tarjeta, manana.AddDays(1));
			estacion.DevolverBici(tarjeta, manana.AddDays(1).AddMinutes(20));
			decimal saldoAntes = tarjeta.Saldo;

			Assert.That(estacion.CalcularCostoRetiro(tarjeta, manana.AddDays(2)), Is.EqualTo(EstacionBicicletas.TarifaDiaria));

			estacion.RetirarBici(tarjeta, manana.AddDays(2));

			Assert.That(tarjeta.Saldo, Is.EqualTo(saldoAntes - EstacionBicicletas.TarifaDiaria));
		}

		[Test]
		public void RetirarBici_MultaGeneradaDespuesDePagarElDia_SeCobraEnElProximoPago()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(10000m);
			UsarBiciConDemora(estacion, tarjeta, manana);

			Assert.That(estacion.CalcularCostoRetiro(tarjeta, manana.AddHours(3)), Is.EqualTo(0m));
			Assert.That(estacion.RetirarBici(tarjeta, manana.AddHours(3)), Is.True);
			Assert.That(estacion.MultasPendientes(tarjeta), Is.EqualTo(1));
			Assert.That(tarjeta.Saldo, Is.EqualTo(10000m - EstacionBicicletas.TarifaDiaria));
		}

		[TestCaseSource(typeof(TiposDeTarjetaFuente), nameof(TiposDeTarjetaFuente.Todos))]
		public void RetirarBici_CualquierTipoDeTarjeta_PagaTarifaCompletaSinFranjaHoraria(TarjetaTipo tipo)
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = new Tarjeta { Id = 7, TarjetaTipo = tipo };
			tarjeta.RecargarSaldo(5000m);
			DateTime sabadoALas23 = new DateTime(2026, 11, 7, 23, 0, 0);

			bool retirada = estacion.RetirarBici(tarjeta, sabadoALas23);

			Assert.That(retirada, Is.True);
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000m - EstacionBicicletas.TarifaDiaria));
		}

		[Test]
		public void RetirarBici_ConSaldoPendienteDeAcreditacion_AcreditaLuegoDelPago()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(30000m, 30000m);

			bool retirada = estacion.RetirarBici(tarjeta, manana);

			Assert.That(retirada, Is.True);
			Assert.That(tarjeta.Saldo, Is.EqualTo(Tarjeta.SaldoMaximo));
			Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(4000m - EstacionBicicletas.TarifaDiaria));
		}

		[Test]
		public void DevolverBici_SinAlquilerEnCurso_DevuelveFalse()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(5000m);

			Assert.That(estacion.DevolverBici(tarjeta, manana), Is.False);
		}

		[Test]
		public void DevolverBici_FechaAnteriorAlRetiro_DevuelveFalseYSigueEnCurso()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(5000m);
			estacion.RetirarBici(tarjeta, manana);

			bool devuelta = estacion.DevolverBici(tarjeta, manana.AddMinutes(-1));

			Assert.That(devuelta, Is.False);
			Assert.That(estacion.Alquileres[0].EstaEnCurso(), Is.True);
		}

		[Test]
		public void TieneBiciEnUso_SegunElEstadoDelAlquiler()
		{
			var estacion = new EstacionBicicletas();
			var tarjeta = CrearTarjetaConSaldo(5000m);

			Assert.That(estacion.TieneBiciEnUso(tarjeta), Is.False);

			estacion.RetirarBici(tarjeta, manana);
			Assert.That(estacion.TieneBiciEnUso(tarjeta), Is.True);

			estacion.DevolverBici(tarjeta, manana.AddMinutes(30));
			Assert.That(estacion.TieneBiciEnUso(tarjeta), Is.False);
		}

		[Test]
		public void MultasPendientes_SoloCuentaLasDeLaMismaTarjeta()
		{
			var estacion = new EstacionBicicletas();
			var tarjetaConMulta = CrearTarjetaConSaldo(10000m);
			var otraTarjeta = new Tarjeta { Id = 2 };
			otraTarjeta.RecargarSaldo(10000m);
			UsarBiciConDemora(estacion, tarjetaConMulta, manana);

			Assert.That(estacion.MultasPendientes(otraTarjeta), Is.EqualTo(0));
			Assert.That(estacion.PagoTarifaDelDia(otraTarjeta, manana), Is.False);
			Assert.That(estacion.CalcularCostoRetiro(otraTarjeta, manana.AddHours(3)), Is.EqualTo(EstacionBicicletas.TarifaDiaria));
		}
	}
}
