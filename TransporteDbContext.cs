using Microsoft.EntityFrameworkCore;

namespace TarjetaSUBE;

public class TransporteDbContext(DbContextOptions<TransporteDbContext> options) : DbContext(options)
{
    public DbSet<Tarjeta> Tarjetas => Set<Tarjeta>();
    public DbSet<Colectivo> Colectivos => Set<Colectivo>();
    public DbSet<Boleto> Boletos => Set<Boleto>();
}
