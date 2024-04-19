using FleetManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
namespace FleetManagementAPI
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<Usuario> Usuarios { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Host=ep-little-pond-a4bjr7b8-pooler.us-east-1.aws.neon.tech;Database=verceldb;Username=default;Password=ymQCBw9DrY1i";
                var connection = new NpgsqlConnection(connectionString);
                optionsBuilder.UseNpgsql(connection);
            }
        }


    }
}