using Microsoft.EntityFrameworkCore;

namespace TarjetaSUBE;

public class TransporteDbContext(DbContextOptions<TransporteDbContext> options) : DbContext(options)
{
    public DbSet<Tarjeta> Tarjetas => Set<Tarjeta>();
    public DbSet<Colectivo> Colectivos => Set<Colectivo>();
    public DbSet<Boleto> Boletos => Set<Boleto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Boleto no tiene una propiedad de navegacion a Tarjeta, asi que EF no detecta
        // la relacion solo: hay que indicarle que TarjetaId es la clave foranea.
        modelBuilder.Entity<Boleto>()
            .HasOne<Tarjeta>()
            .WithMany()
            .HasForeignKey(b => b.TarjetaId);
    }
}
