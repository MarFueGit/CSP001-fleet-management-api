using FleetManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementAPI.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IEnumerable<Taxi>> Get() => await _context.Taxis.ToListAsync();
    }
}
