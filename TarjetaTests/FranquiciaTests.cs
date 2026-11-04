using System;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	public class FranquiciaTests
	{
		// Lunes 2 de noviembre de 2026 a las 10:00 (dia habil dentro de la franja 6 a 22)
		private readonly DateTime lunesALas10 = new DateTime(2026, 11, 2, 10, 0, 0);

		[Test]
		public void PagarCon_BoletoGratuito_PermiteHastaDosViajesGratisPorDiaYTercerViajeCobraPrecioCompleto()
		{
			// Resultado esperado: El boleto gratuito estudiantil permite hasta 2 viajes sin cargo ($0) por dia,
			// y a partir del 3er viaje del mismo dia cobra la tarifa completa ($2150).
			var tarjeta = new Tarjeta
			{
				Id = 50,
				NumeroTarjeta = "50505050",
				TarjetaTipo = new BoletoGratuito()
			};
			tarjeta.RecargarSaldo(3000);

			var colectivo = new Colectivo();

			Assert.That(colectivo.PagarCon(tarjeta, lunesALas10), Is.True);
			Assert.That(tarjeta.Boletos[0].TotalAbonado, Is.EqualTo(0m));

			Assert.That(colectivo.PagarCon(tarjeta, lunesALas10.AddMinutes(10)), Is.True);
			Assert.That(tarjeta.Boletos[1].TotalAbonado, Is.EqualTo(0m));

			Assert.That(colectivo.PagarCon(tarjeta, lunesALas10.AddMinutes(20)), Is.True);
			Assert.That(tarjeta.Boletos[2].TotalAbonado, Is.EqualTo(2150m));
			Assert.That(tarjeta.Saldo, Is.EqualTo(3000m - 2150m));
		}

		[TestCase(typeof(MedioBoleto))]
		[TestCase(typeof(BoletoGratuito))]
		[TestCase(typeof(FranquiciaCompleta))]
		public void PagarCon_FranquiciasDeLunesAViernesDe6A22_PermiteViajar(Type tipoFranquicia)
		{
			// Resultado esperado: Todas las franquicias pueden viajar de lunes a viernes dentro del horario de 6:00 a 22:00.
			var colectivo = new Colectivo();

			// Probamos de Lunes (2/11/2026) a Viernes (6/11/2026) a las 6:00 y a las 21:59
			for (int dia = 2; dia <= 6; dia++)
			{
				var tarjetaManiana = new Tarjeta
				{
					TarjetaTipo = (TarjetaTipo)Activator.CreateInstance(tipoFranquicia)!
				};
				tarjetaManiana.RecargarSaldo(3000);

				var tarjetaNoche = new Tarjeta
				{
					TarjetaTipo = (TarjetaTipo)Activator.CreateInstance(tipoFranquicia)!
				};
				tarjetaNoche.RecargarSaldo(3000);

				DateTime aLas6 = new DateTime(2026, 11, dia, 6, 0, 0);
				DateTime aLas21y59 = new DateTime(2026, 11, dia, 21, 59, 0);

				Assert.That(colectivo.PagarCon(tarjetaManiana, aLas6), Is.True);
				Assert.That(colectivo.PagarCon(tarjetaNoche, aLas21y59), Is.True);
			}
		}

		[TestCase(typeof(MedioBoleto))]
		[TestCase(typeof(BoletoGratuito))]
		[TestCase(typeof(FranquiciaCompleta))]
		public void PagarCon_FranquiciasAntesDeLas6ODespuesDeLas22_NoPermiteViajar(Type tipoFranquicia)
		{
			// Resultado esperado: Ninguna franquicia permite pagar un boleto antes de las 6:00 ni a partir de las 22:00
			// (devuelve false y no genera boleto).
			var tarjeta = new Tarjeta
			{
				TarjetaTipo = (TarjetaTipo)Activator.CreateInstance(tipoFranquicia)!
			};
			tarjeta.RecargarSaldo(3000);

			var colectivo = new Colectivo();

			DateTime antesDeLas6 = new DateTime(2026, 11, 2, 5, 59, 0);
			DateTime aLas22 = new DateTime(2026, 11, 2, 22, 0, 0);
			DateTime aLas23 = new DateTime(2026, 11, 2, 23, 30, 0);

			Assert.That(colectivo.PagarCon(tarjeta, antesDeLas6), Is.False);
			Assert.That(colectivo.PagarCon(tarjeta, aLas22), Is.False);
			Assert.That(colectivo.PagarCon(tarjeta, aLas23), Is.False);
			Assert.That(tarjeta.Boletos, Is.Empty);
		}

		[TestCase(typeof(MedioBoleto))]
		[TestCase(typeof(BoletoGratuito))]
		[TestCase(typeof(FranquiciaCompleta))]
		public void PagarCon_FranquiciasEnSabadoODomingo_NoPermiteViajar(Type tipoFranquicia)
		{
			// Resultado esperado: Ninguna franquicia permite viajar los sabados ni los domingos,
			// aun estando dentro del horario de 6:00 a 22:00 (devuelve false y no genera boleto).
			var tarjeta = new Tarjeta
			{
				TarjetaTipo = (TarjetaTipo)Activator.CreateInstance(tipoFranquicia)!
			};
			tarjeta.RecargarSaldo(3000);

			var colectivo = new Colectivo();

			// Sabado 7 y Domingo 8 de noviembre de 2026 a las 12:00
			DateTime sabado = new DateTime(2026, 11, 7, 12, 0, 0);
			DateTime domingo = new DateTime(2026, 11, 8, 12, 0, 0);

			Assert.That(colectivo.PagarCon(tarjeta, sabado), Is.False);
			Assert.That(colectivo.PagarCon(tarjeta, domingo), Is.False);
			Assert.That(tarjeta.Boletos, Is.Empty);
		}

		[Test]
		public void PagarCon_TarjetaNormalFueraDeHorarioYFinDeSemana_SiemprePermiteViajar()
		{
			// Resultado esperado: La tarjeta Normal no tiene restriccion horaria y puede viajar
			// tanto de madrugada como los fines de semana.
			var tarjeta = new Tarjeta { TarjetaTipo = new Normal() };
			tarjeta.RecargarSaldo(10000);

			var colectivo = new Colectivo();

			DateTime madrugada = new DateTime(2026, 11, 2, 3, 0, 0);
			DateTime sabado = new DateTime(2026, 11, 7, 23, 0, 0);
			DateTime domingo = new DateTime(2026, 11, 8, 5, 0, 0);

			Assert.That(colectivo.PagarCon(tarjeta, madrugada), Is.True);
			Assert.That(colectivo.PagarCon(tarjeta, sabado), Is.True);
			Assert.That(colectivo.PagarCon(tarjeta, domingo), Is.True);
			Assert.That(tarjeta.Boletos, Has.Count.EqualTo(3));
		}
	}
}
