using Microsoft.EntityFrameworkCore;

namespace TarjetaSUBE;

public class TransporteDbContext(DbContextOptions<TransporteDbContext> options) : DbContext(options)
{
    public DbSet<Tarjeta> Tarjetas => Set<Tarjeta>();
    public DbSet<Colectivo> Colectivos => Set<Colectivo>();
    public DbSet<Boleto> Boletos => Set<Boleto>();
    public DbSet<TarjetaTipo> TarjetasTipo => Set<TarjetaTipo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Una Tarjeta tiene muchos Boletos (Tarjeta.Boletos). Boleto no tiene una propiedad
        // de navegacion a Tarjeta, asi que hay que indicarle que TarjetaId es la clave foranea.
        modelBuilder.Entity<Boleto>()
            .HasOne<Tarjeta>()
            .WithMany(t => t.Boletos)
            .HasForeignKey(b => b.TarjetaId);

        // Tarjeta tiene una clave foranea a TarjetaTipo
        modelBuilder.Entity<Tarjeta>()
            .HasOne(t => t.TarjetaTipo)
            .WithMany()
            .HasForeignKey(t => t.TarjetaTipoId);

        modelBuilder.Entity<ColectivoInterurbano>();
    }
}
