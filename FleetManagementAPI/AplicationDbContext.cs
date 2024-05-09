using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FleetManagementAPI.Models
{
    public interface IDbContext
    {
        DbSet<Taxi> Taxis { get; set; }
        DbSet<Trajectorie> Trajectories { get; set; }

        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync();
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
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura la clave principal para la entidad Taxi
            modelBuilder.Entity<Taxi>()
                .HasKey(t => t.idtaxi);

            // Configurar clave primaria para entidad Trajectoria
            modelBuilder.Entity<Trajectorie>()
                .HasKey(t => t.idtrajectorie);
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
