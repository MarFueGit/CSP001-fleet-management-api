using FleetManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FleetManagementAPI.Controllers
{
    //[Authorize]
    [Route("api/taxis")]
    [ApiController]
    public class TaxisController : ControllerBase
    {
        private readonly IDbContext _context;

        public TaxisController(IDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationMetadata))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Description("Get the list of taxis")]
        public async Task<ActionResult<object>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var totalCount = await _context.Taxis.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var taxis = await _context.Taxis
                    .OrderBy(t => t.idtaxi)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new { t.idtaxi, t.plate })
                    .ToListAsync();

                var paginationMetadata = new PaginationMetadata
                {
                    total = totalCount,
                    totalPages = totalPages,
                    currentPage = page,
                    nextPage = page < totalPages ? page + 1 : (int?)null,
                    lastPage = totalPages,
                    data = taxis
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
