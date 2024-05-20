
using System.ComponentModel.DataAnnotations;

namespace FleetManagementAPI.Models
{
    // Modelo usuario
    public class User
    {
        [Key] // Define la propiedad como clave primaria
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }

    // DTO para la creacion de usuarios
    public class CreateUserDto
    {
        [Required] // Campo obligatorio
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }

    // DTO para la actualizacion de usuarios
    public class UpdateUserDto
    {
        public string Name { get; set; } 

        [EmailAddress] // Debe ser una dirección de correo electrónico válida
        public string Email { get; set; } // Nueva dirección de correo electrónico del usuario

        public string Role { get; set; } 
    }

    // DTO para iniciar sesion
    public class LoginDto
    {
        [Required] 
        [EmailAddress] 
        public string email { get; set; } 
        [Required]
        public string password { get; set; } // Contraseña del usuario
    }



}