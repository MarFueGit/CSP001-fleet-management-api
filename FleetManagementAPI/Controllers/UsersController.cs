using FleetManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FleetManagementAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDbContext _context;

        public UsersController(IDbContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationMetadata))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var totalCount = await _context.Usuarios.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var users = await _context.Usuarios
                    .OrderBy(u => u.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new { u.Id, u.Nombre, u.CorreoElectronico })
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
    }
}
