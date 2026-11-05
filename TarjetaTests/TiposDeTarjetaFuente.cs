using System.Collections.Generic;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	public static class TiposDeTarjetaFuente
	{
		public static IEnumerable<TestCaseData> Todos()
		{
			yield return Caso(TarjetaTipo.Normal());
			foreach (TestCaseData caso in Franquicias())
			{
				yield return caso;
			}
		}

		public static IEnumerable<TestCaseData> Franquicias()
		{
			yield return Caso(TarjetaTipo.MedioBoleto());
			foreach (TestCaseData caso in FranquiciasGratuitas())
			{
				yield return caso;
			}
		}

		public static IEnumerable<TestCaseData> FranquiciasGratuitas()
		{
			yield return Caso(TarjetaTipo.BoletoGratuito());
			yield return Caso(TarjetaTipo.FranquiciaCompleta());
		}

		private static TestCaseData Caso(TarjetaTipo tipo)
		{
			return new TestCaseData(tipo).SetArgDisplayNames(tipo.Nombre);
		}
	}
}
