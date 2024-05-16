//Importacion de espacio de nombres

using FleetManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace FleetManagementAPI.Controllers
{
    [Route("api/users")] // Establece la ruta base para todas acciones en este controlador.
    [ApiController] // Indica que este controlador responde a las solicitudes web API.
    public class UsersController : ControllerBase
    {
        private readonly IDbContext _context;
        private readonly HashPassword _hashPassword;

        public UsersController(IDbContext context, IConfiguration configuration)
        {
            _context = context;
            _hashPassword = new HashPassword(configuration);
        }

        public UsersController(IDbContext @object)
        {
        }

        /// <summary>
        /// Retrieves a paginated list of users.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <returns>A paginated list of users.</returns>
        [HttpGet(Name = "users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationMetadata))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")] // Add this line to specify that the endpoint produces JSON
        [Description("Return list of users")]
        public async Task<ActionResult<object>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var totalCount = await _context.Users.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var users = await _context.Users
                    .OrderBy(u => u.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new { u.Id, u.Name, u.Email, u.Role })
                    .ToListAsync();

                var paginationMetadata = new PaginationMetadata
                {
                    total = totalCount,
                    totalPages = totalPages,
                    currentPage = page,
                    nextPage = page < totalPages ? page + 1 : (int?)null,
                    lastPage = totalPages,
                    data = users
                };

                return Ok(paginationMetadata);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="createUserDto">The data to create the user.</param>
        /// <returns>The newly created user.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Description("Create a new user")]
        public async Task<ActionResult<User>> PostUsuario([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                // L�gica para crear un nuevo usuario

                // Comprobamos si ya existe un usuario con el mismo correo electr�nico
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == createUserDto.Email);
                if (existingUser != null)
                {
                    return BadRequest("User with this email already exists.");
                }

                // Hash the password
                string hashedPassword = _hashPassword.EncryptPassword(createUserDto.Password);

                // Creamos un nuevo objeto User
                var newUser = new User
                {
                    Name = createUserDto.Name,
                    Email = createUserDto.Email,
                    Role = createUserDto.Role,
                    Password = hashedPassword // Guardamos la contrase�a cifrada
                };

                // A�adimos el nuevo usuario a la base de datos
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Devolvemos un CreatedAtActionResult con el nuevo usuario creado
                return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="updateUserDto">The data to update the user.</param>
        /// <returns>The updated user.</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")] // Add this line to specify that the endpoint produces JSON
        [Description("Update the user by id")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                // Find the user by ID
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                // Update user properties if provided
                if (!string.IsNullOrEmpty(updateUserDto.name))
                {
                    user.Name = updateUserDto.name;
                }

                if (!string.IsNullOrEmpty(updateUserDto.email))
                {
                    user.Email = updateUserDto.email;
                }

                if (!string.IsNullOrEmpty(updateUserDto.role))
                {
                    user.Role = updateUserDto.role;
                }

                // Apply changes to the entity
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Deletes an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")] // Add this line to specify that the endpoint produces JSON
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Description("Delete a user by id")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                // Find the user by ID
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                // Remove the user from the database
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
