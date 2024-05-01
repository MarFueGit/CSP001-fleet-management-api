using Microsoft.EntityFrameworkCore;
using FleetManagementAPI.Models;

namespace FleetManagementAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Taxi> Taxis { get; set; } // Migracion Taxi
        public DbSet<Trajectorie> Trajectories { get; set; } // Migracion Trajectorie

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
