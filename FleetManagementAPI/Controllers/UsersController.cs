using FleetManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FleetManagementAPI.Controllers
{
    [Route("api/users")] // Define la ruta base para las acciones del controlador
    [ApiController] // Indica que esta clase es un controlador de API
    public class UsersController : ControllerBase
    {
        private readonly IDbContext _context;
        private readonly IHashPassword _hashPassword;

        public UsersController(IDbContext context, IHashPassword hashPassword)
        {
            _context = context;
            _hashPassword = hashPassword;
        }

        [HttpGet(Name = "users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationMetadata))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Description("Return list of users")]
        public async Task<ActionResult<object>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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
                return HandleError(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Description("Create a new user")]
        public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == createUserDto.Email);
                if (existingUser != null)
                {
                    return BadRequest("User with this email already exists.");
                }

                string hashedPassword = _hashPassword.EncryptPassword(createUserDto.Password);

                var newUser = new User
                {
                    Name = createUserDto.Name,
                    Email = createUserDto.Email,
                    Role = createUserDto.Role,
                    Password = hashedPassword
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUsers), new { id = newUser.Id }, newUser);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Description("Update the user by id")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                user.Name = updateUserDto.Name ?? user.Name;
                user.Email = updateUserDto.Email ?? user.Email;
                user.Role = updateUserDto.Role ?? user.Role;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Description("Delete a user by id")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        private ActionResult HandleError(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
