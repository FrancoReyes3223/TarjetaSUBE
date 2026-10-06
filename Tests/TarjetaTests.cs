using NUnit.Framework;
using TarjetaSUBE; 

namespace TarjetaSUBE.Tests;

public class TarjetaTests
{
	
	[TestCase(2000)]

	[TestCase(3000)]

	[TestCase(4000)]

	[TestCase(5000)]

	[TestCase(8000)]

	[TestCase(10000)]

	[TestCase(15000)]

	[TestCase(20000)]

	[TestCase(25000)]

	[TestCase(30000)]

	public void RecargarSaldo_MontosValidos_SumaElSaldoYDevuelveTrue(int montoRecarga)
	{
		
		var tarjeta = new Tarjeta();
		
		bool resultado = tarjeta.RecargarSaldo(montoRecarga);

		Assert.That(resultado, Is.True); 
		Assert.That(tarjeta.Saldo, Is.EqualTo(montoRecarga)); 
	}

	[Test] 

	public void RecargarSaldo_MontoInvalido_DevuelveFalse()
	{
		var tarjeta = new Tarjeta();
		bool resultado = tarjeta.RecargarSaldo(1500); 
		Assert.That(resultado, Is.False);
	}

	[Test] 
	public void RecargarSaldo_MontoMayorAlLimite_DevuelveFalse()
	{
		var tarjeta = new Tarjeta();
		bool resultado = tarjeta.RecargarSaldo(45000); 
		Assert.That(resultado, Is.False);
	}

	[Test]
	public void PagarCon_ConSaldoSuficiente_DevuelveBoletoYDescuentaSaldo()
	{
		
		var tarjeta = new Tarjeta();
		tarjeta.Id = 1;
		tarjeta.NumeroTarjeta = 6061267008306639;
		tarjeta.RecargarSaldo(3000);

		var colectivo = new Colectivo(); 

		Boleto boletoGenerado = colectivo.PagarCon(tarjeta);

		Assert.That(boletoGenerado, Is.Not.Null);

		Assert.That(boletoGenerado.MontoAbonado, Is.EqualTo(1580));

		Assert.That(boletoGenerado.TarjetaId, Is.EqualTo(tarjeta.Id));

		Assert.That(tarjeta.Saldo, Is.EqualTo(1420));
	}

	[Test]
	public void PagarCon_ConSaldoInSuficiente_DevuelveNull()
	{

		var tarjeta = new Tarjeta();
		tarjeta.Id = 2;
		tarjeta.NumeroTarjeta = 6061267008306632;

		var colectivo = new Colectivo();

		Boleto boletoGenerado = colectivo.PagarCon(tarjeta);

		Assert.That(boletoGenerado, Is.Null);

	}

}