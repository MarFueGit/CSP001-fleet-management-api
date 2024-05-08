using Microsoft.EntityFrameworkCore;

namespace FleetManagementAPI.Models
{
    public interface IDbContext
    {
        DbSet<Taxi> Taxis { get; set; }
        DbSet<Trajectorie> Trajectories { get; set; }

        DbSet<Usuario> Usuarios { get; set; }
        int SaveChanges();
    }

    public class ApplicationDbContext : DbContext, IDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Constructor sin parametros para pruebas.
        public ApplicationDbContext() : base(new DbContextOptions<ApplicationDbContext>())
        {
        }

        public virtual DbSet<Taxi> Taxis { get; set; }
        public DbSet<Trajectorie> Trajectories { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura la clave principal para la entidad Taxi
            modelBuilder.Entity<Taxi>()
                .HasKey(t => t.idtaxi);

            // Configurar clave primaria para entidad Trajectoria
            modelBuilder.Entity<Trajectorie>()
                .HasKey(t => t.idtrajectorie);
        }
    }
}

