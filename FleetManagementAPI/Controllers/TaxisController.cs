using FleetManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FleetManagementAPI.Controllers
{
    [Route("api/taxis")]
    [ApiController]
    public class TaxisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaxisController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of taxis with pagination.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>A paginated list of taxis.</returns>
        /// <response code="200">Returns the paginated list of taxis.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet(Name = "taxis")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationMetadata))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
