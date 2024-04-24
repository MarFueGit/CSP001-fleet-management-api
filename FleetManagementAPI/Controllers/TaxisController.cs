using FleetManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

        // GET: /<TaxisController>
        [HttpGet(Name = "taxis")]
        public async Task<ActionResult<object>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var totalCount = await _context.Taxis.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var taxis = await _context.Taxis
                .OrderBy(t => t.idtaxi) // Change this to your sorting criteria
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paginationMetadata = new
            {
                total = totalCount,
                totalPages,
                currentPage = page,
                nextPage = page < totalPages ? page + 1 : (int?)null,
                lastPage = totalPages,
                data = taxis
            };

            // Serialize paginationMetadata using System.Text.Json
            var jsonMetadata = JsonSerializer.Serialize(paginationMetadata);

            Response.Headers.Add("X-Pagination", jsonMetadata);

            return paginationMetadata;
        }
    }
}
