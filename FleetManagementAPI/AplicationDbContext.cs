using Microsoft.EntityFrameworkCore;
using FleetManagementAPI.Models;
using System.Linq;

namespace FleetManagementAPI
{
    public interface IDbContext
    {
        DbSet<Taxi> Taxis { get; set; }
        DbSet<Trajectorie> Trajectories { get; set; }
        int SaveChanges();
    }

    public class ApplicationDbContext : DbContext, IDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Taxi> Taxis { get; set; }
        public DbSet<Trajectorie> Trajectories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura la clave principal para la entidad Taxi
            modelBuilder.Entity<Taxi>()
                .HasKey(t => t.idtaxi);

            // Configura la clave principal para la entidad Trajectoria
            modelBuilder.Entity<Trajectorie>()
                .HasKey(t => t.idtrajectorie); // Suponiendo que Id es la propiedad que representa la clave principal
        }
    }
}
