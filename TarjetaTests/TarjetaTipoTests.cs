using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	public class TarjetaTipoTests
	{
		private readonly DateTime martesALas10 = new DateTime(2026, 11, 3, 10, 0, 0);
		private readonly HistorialViajes historialVacio = new HistorialViajes(new List<Boleto>());

		[Test]
		public void Normal_NoEsFranquiciaYPagaTarifaCompleta()
		{
			var tipo = TarjetaTipo.Normal();

			Assert.That(tipo.Nombre, Is.EqualTo("Normal"));
			Assert.That(tipo.EsFranquicia, Is.False);
			Assert.That(tipo.PorcentajeTarifa, Is.EqualTo(100));
			Assert.That(tipo.ViajesDiariosConFranquicia, Is.Null);
			Assert.That(tipo.MinutosEntreViajes, Is.Null);
		}

		[Test]
		public void MedioBoleto_TieneMitadDeTarifaLimiteDiarioYMinutosEntreViajes()
		{
			var tipo = TarjetaTipo.MedioBoleto();

			Assert.That(tipo.EsFranquicia, Is.True);
			Assert.That(tipo.PorcentajeTarifa, Is.EqualTo(50));
			Assert.That(tipo.ViajesDiariosConFranquicia, Is.EqualTo(2));
			Assert.That(tipo.MinutosEntreViajes, Is.EqualTo(5));
		}

		[Test]
		public void BoletoGratuito_EsGratisConLimiteDiario()
		{
			var tipo = TarjetaTipo.BoletoGratuito();

			Assert.That(tipo.EsFranquicia, Is.True);
			Assert.That(tipo.PorcentajeTarifa, Is.EqualTo(0));
			Assert.That(tipo.ViajesDiariosConFranquicia, Is.EqualTo(2));
			Assert.That(tipo.MinutosEntreViajes, Is.Null);
		}

		[Test]
		public void FranquiciaCompleta_EsGratisSinLimiteDiario()
		{
			var tipo = TarjetaTipo.FranquiciaCompleta();

			Assert.That(tipo.EsFranquicia, Is.True);
			Assert.That(tipo.PorcentajeTarifa, Is.EqualTo(0));
			Assert.That(tipo.ViajesDiariosConFranquicia, Is.Null);
		}

		[Test]
		public void CalcularTarifa_AplicaElPorcentajeDelTipo()
		{
			var tipo = new TarjetaTipo { PorcentajeTarifa = 30 };

			Assert.That(tipo.CalcularTarifa(2150m), Is.EqualTo(645m));
		}

		[Test]
		public void CalcularTarifa_FranquiciaCompletaSinLimite_SiempreGratis()
		{
			var boletos = new List<Boleto>();
			for (int i = 0; i < 10; i++)
			{
				boletos.Add(new Boleto { FechayHora = martesALas10.AddMinutes(-i - 1) });
			}

			decimal tarifa = TarjetaTipo.FranquiciaCompleta().CalcularTarifa(2150m, new HistorialViajes(boletos), martesALas10);

			Assert.That(tarifa, Is.EqualTo(0m));
		}

		[Test]
		public void PuedeViajar_TipoNoFranquicia_SinRestriccionHoraria()
		{
			var tipo = new TarjetaTipo { Nombre = "Personalizada", PorcentajeTarifa = 100 };
			DateTime domingoALas3 = new DateTime(2026, 11, 8, 3, 0, 0);

			Assert.That(tipo.PuedeViajar(historialVacio, domingoALas3), Is.True);
		}

		[Test]
		public void PuedeViajar_FranquiciaNueva_RespetaFranjaHoraria()
		{
			var tipo = new TarjetaTipo { Nombre = "Jubilado provincial", PorcentajeTarifa = 0, EsFranquicia = true };

			Assert.That(tipo.PuedeViajar(historialVacio, martesALas10), Is.True);
			Assert.That(tipo.PuedeViajar(historialVacio, new DateTime(2026, 11, 3, 22, 0, 0)), Is.False);
			Assert.That(tipo.PuedeViajar(historialVacio, new DateTime(2026, 11, 7, 10, 0, 0)), Is.False);
		}
	}
}
