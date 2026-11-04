using System;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	public class TrasbordoTests
	{
		// Lunes 2 de noviembre de 2026 a las 10:00 (dia habil y horario valido)
		private readonly DateTime lunesALas10 = new DateTime(2026, 11, 2, 10, 0, 0);

		[Test]
		public void PagarCon_EntreLineasDistintasDentroDeUnaHora_EsTrasbordoSinCostoYSeIndicaEnElBoleto()
		{
			// Resultado esperado: El 1er viaje abona tarifa normal ($2150). El 2do viaje (a los 30 min en otra linea)
			// es reconocido como trasbordo gratuito ($0, EsTrasbordo = true) y el saldo solo disminuye por el primer viaje.
			var tarjeta = new Tarjeta();
			tarjeta.RecargarSaldo(5000);

			var lineaK = new Colectivo { Linea = "K" };
			var linea102 = new Colectivo { Linea = "102" };

			Assert.That(lineaK.PagarCon(tarjeta, lunesALas10), Is.True);
			Assert.That(linea102.PagarCon(tarjeta, lunesALas10.AddMinutes(30)), Is.True);

			Assert.That(tarjeta.Boletos[0].EsTrasbordo, Is.False);
			Assert.That(tarjeta.Boletos[0].TotalAbonado, Is.EqualTo(2150m));

			Assert.That(tarjeta.Boletos[1].EsTrasbordo, Is.True);
			Assert.That(tarjeta.Boletos[1].TotalAbonado, Is.EqualTo(0m));
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000m - 2150m));
		}

		[Test]
		public void PagarCon_MultiplesTrasbordosDentroDeUnaHoraDesdeElPrimerBoleto_TodosSonSinCosto()
		{
			// Resultado esperado: Multiples trasbordos en lineas distintas dentro de los 60 minutos desde que se pago
			// el primer pasaje son todos sin costo ($0, EsTrasbordo = true). Solo se cobra el viaje inicial.
			var tarjeta = new Tarjeta();
			tarjeta.RecargarSaldo(5000);

			var lineaK = new Colectivo { Linea = "K" };
			var linea102 = new Colectivo { Linea = "102" };
			var linea133 = new Colectivo { Linea = "133" };
			var linea144 = new Colectivo { Linea = "144" };

			lineaK.PagarCon(tarjeta, lunesALas10);
			linea102.PagarCon(tarjeta, lunesALas10.AddMinutes(15));
			linea133.PagarCon(tarjeta, lunesALas10.AddMinutes(35));
			linea144.PagarCon(tarjeta, lunesALas10.AddMinutes(60));

			Assert.That(tarjeta.Boletos[0].EsTrasbordo, Is.False);
			Assert.That(tarjeta.Boletos[1].EsTrasbordo, Is.True);
			Assert.That(tarjeta.Boletos[2].EsTrasbordo, Is.True);
			Assert.That(tarjeta.Boletos[3].EsTrasbordo, Is.True);
			Assert.That(tarjeta.Boletos[1].TotalAbonado, Is.EqualTo(0m));
			Assert.That(tarjeta.Boletos[2].TotalAbonado, Is.EqualTo(0m));
			Assert.That(tarjeta.Boletos[3].TotalAbonado, Is.EqualTo(0m));
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000m - 2150m));
		}

		[Test]
		public void PagarCon_PasadaUnaHoraDesdeElPrimerBoleto_NoEsTrasbordoYCobraTarifaNormal()
		{
			// Resultado esperado: El viaje de los 45 min es trasbordo ($0). Al pasar 65 minutos del primer viaje pagado,
			// expiro la ventana de 1 hora, por lo que el siguiente viaje se cobra con tarifa completa ($2150, EsTrasbordo = false).
			var tarjeta = new Tarjeta();
			tarjeta.RecargarSaldo(5000);

			var lineaK = new Colectivo { Linea = "K" };
			var linea102 = new Colectivo { Linea = "102" };
			var linea133 = new Colectivo { Linea = "133" };

			lineaK.PagarCon(tarjeta, lunesALas10);
			linea102.PagarCon(tarjeta, lunesALas10.AddMinutes(45));
			linea133.PagarCon(tarjeta, lunesALas10.AddMinutes(65));

			Assert.That(tarjeta.Boletos[1].EsTrasbordo, Is.True);
			Assert.That(tarjeta.Boletos[2].EsTrasbordo, Is.False);
			Assert.That(tarjeta.Boletos[2].TotalAbonado, Is.EqualTo(2150m));
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000m - 2150m - 2150m));
		}

		[Test]
		public void PagarCon_MismaLineaDentroDeUnaHora_NoEsTrasbordo()
		{
			// Resultado esperado: Si se vuelve a pagar en la misma linea de colectivo dentro de la hora,
			// no cuenta como trasbordo y se cobra la tarifa normal ($2150, EsTrasbordo = false).
			var tarjeta = new Tarjeta();
			tarjeta.RecargarSaldo(5000);

			var lineaK = new Colectivo { Linea = "K" };

			lineaK.PagarCon(tarjeta, lunesALas10);
			lineaK.PagarCon(tarjeta, lunesALas10.AddMinutes(20));

			Assert.That(tarjeta.Boletos[1].EsTrasbordo, Is.False);
			Assert.That(tarjeta.Boletos[1].TotalAbonado, Is.EqualTo(2150m));
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000m - 2150m - 2150m));
		}

		[Test]
		public void PagarCon_SabadoEnHorarioValido_PermiteTrasbordo()
		{
			// Resultado esperado: Los dias sabados dentro del rango de 7:00 a 22:00 el trasbordo
			// entre lineas distintas dentro de la hora es valido y gratuito ($0, EsTrasbordo = true).
			var tarjeta = new Tarjeta();
			tarjeta.RecargarSaldo(5000);

			DateTime sabadoMediodia = new DateTime(2026, 11, 7, 12, 0, 0);

			new Colectivo { Linea = "K" }.PagarCon(tarjeta, sabadoMediodia);
			new Colectivo { Linea = "102" }.PagarCon(tarjeta, sabadoMediodia.AddMinutes(25));

			Assert.That(tarjeta.Boletos[1].EsTrasbordo, Is.True);
			Assert.That(tarjeta.Boletos[1].TotalAbonado, Is.EqualTo(0m));
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000m - 2150m));
		}

		[Test]
		public void PagarCon_Domingo_NoPermiteTrasbordo()
		{
			// Resultado esperado: Los domingos no rige el beneficio de trasbordo; el segundo viaje en otra linea
			// se cobra con tarifa completa ($2150, EsTrasbordo = false).
			var tarjeta = new Tarjeta();
			tarjeta.RecargarSaldo(5000);

			DateTime domingoMediodia = new DateTime(2026, 11, 8, 12, 0, 0);

			new Colectivo { Linea = "K" }.PagarCon(tarjeta, domingoMediodia);
			new Colectivo { Linea = "102" }.PagarCon(tarjeta, domingoMediodia.AddMinutes(20));

			Assert.That(tarjeta.Boletos[1].EsTrasbordo, Is.False);
			Assert.That(tarjeta.Boletos[1].TotalAbonado, Is.EqualTo(2150m));
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000m - 2150m - 2150m));
		}

		[TestCase(6, 50)]
		[TestCase(22, 0)]
		[TestCase(23, 15)]
		public void PagarCon_FueraDeFranjaHorariaDe7A22_NoPermiteTrasbordo(int hora, int minuto)
		{
			// Resultado esperado: Fuera de la franja horaria permitida (7:00 a 22:00), no se aplican trasbordos gratuitos
			// y el segundo viaje se abona con tarifa completa ($2150, EsTrasbordo = false).
			var tarjeta = new Tarjeta();
			tarjeta.RecargarSaldo(5000);

			DateTime fechaFueraDeHorario = new DateTime(2026, 11, 2, hora, minuto, 0);

			new Colectivo { Linea = "K" }.PagarCon(tarjeta, fechaFueraDeHorario.AddMinutes(-15));
			new Colectivo { Linea = "102" }.PagarCon(tarjeta, fechaFueraDeHorario);

			Assert.That(tarjeta.Boletos[1].EsTrasbordo, Is.False);
			Assert.That(tarjeta.Boletos[1].TotalAbonado, Is.EqualTo(2150m));
			Assert.That(tarjeta.Saldo, Is.EqualTo(5000m - 2150m - 2150m));
		}

		[TestCase(typeof(Normal))]
		[TestCase(typeof(MedioBoleto))]
		[TestCase(typeof(BoletoGratuito))]
		[TestCase(typeof(FranquiciaCompleta))]
		public void PagarCon_TodosLosTiposDeTarjeta_PuedenRealizarTrasbordo(Type tipoTarjeta)
		{
			// Resultado esperado: Todas las tarjetas sin excepcion pueden realizar trasbordos gratuitos ($0, EsTrasbordo = true)
			// entre lineas distintas dentro de la hora y franja permitida.
			var tarjeta = new Tarjeta
			{
				TarjetaTipo = (TarjetaTipo)Activator.CreateInstance(tipoTarjeta)!
			};
			tarjeta.RecargarSaldo(5000);

			new Colectivo { Linea = "K" }.PagarCon(tarjeta, lunesALas10);
			new Colectivo { Linea = "102" }.PagarCon(tarjeta, lunesALas10.AddMinutes(20));

			Assert.That(tarjeta.Boletos[0].EsTrasbordo, Is.False);
			Assert.That(tarjeta.Boletos[1].EsTrasbordo, Is.True);
			Assert.That(tarjeta.Boletos[1].TotalAbonado, Is.EqualTo(0m));
		}
	}
}
