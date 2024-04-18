using Microsoft.EntityFrameworkCore;

namespace FleetManagementAPI.Models
{
    // Modelo
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string CorreoElectronico { get; set; }
    }

    // Creación de la tabla en la base de datos
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
