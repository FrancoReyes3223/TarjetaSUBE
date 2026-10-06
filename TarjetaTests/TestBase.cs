using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace TarjetaSUBE.Tests
{
	// Clase base para los tests que necesitan base de datos.
	// Cada test recibe una base en memoria nueva y vacia, asi no dependen de SQL Server
	// ni se pisan los datos entre tests. Para usarla: public class MisTests : TestBase
	public abstract class TestBase
	{
		protected TransporteDbContext Db { get; private set; } = null!;

		[SetUp]
		public void CrearBaseEnMemoria()
		{
			var opciones = new DbContextOptionsBuilder<TransporteDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			Db = new TransporteDbContext(opciones);
		}

		[TearDown]
		public void CerrarBase()
		{
			Db.Dispose();
		}
	}
}
