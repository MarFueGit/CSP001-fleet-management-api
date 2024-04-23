using Microsoft.EntityFrameworkCore;
using FleetManagementAPI.Models;

namespace FleetManagementAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; } // Migracion Usuario
        public DbSet<Taxi> Taxis { get; set; } // Migracion Taxi
        public DbSet<Trajectorie> Trajectories { get; set; } // Migracion Trajectorie

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure primary key for Taxi entity
            modelBuilder.Entity<Taxi>()
                .HasKey(t => t.idtaxi);

            // Configure primary key for Trajectorie entity
            modelBuilder.Entity<Trajectorie>()
                .HasKey(t => t.idtrajectorie); // Assuming Id is the property representing the primary key
        }
    }
}
