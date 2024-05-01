using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FleetManagementAPI.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FleetManagementAPI.Controllers
{
    [Route("api/trajectories")]
    [ApiController]
    public class TrajectoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrajectoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("/trajectories")]
        public async Task<ActionResult<object>> SearchTrajectories(int idtaxi, DateTime date, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // Extract the date part from the DateTime object
            var searchDate = date.Date;

            // Query the database for trajectories matching the given idtaxi and date
            var query = _context.Trajectories
                .Where(t => t.taxi_id == idtaxi &&
                            t.date.Year == searchDate.Year &&
                            t.date.Month == searchDate.Month &&
                            t.date.Day == searchDate.Day)
                .OrderByDescending(t => t.date);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var trajectories = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new
                {
                    idtrajectorie = t.idtrajectorie,
                    taxi_id = t.taxi_id,
                    latitude = t.latitude,
                    longitude = t.longitude,
                    timestamp = t.date
                })
                .ToListAsync();

            var paginationMetadata = new
            {
                total = totalCount,
                totalPages,
                currentPage = page,
                nextPage = page < totalPages ? page + 1 : (int?)null,
                lastPage = totalPages,
                data = trajectories
            };

            // Serialize paginationMetadata using System.Text.Json
            var jsonMetadata = JsonSerializer.Serialize(paginationMetadata);

            Response.Headers.Add("X-Pagination", jsonMetadata);

            return paginationMetadata;
        }

        [HttpGet("/last-locations")]
        public async Task<ActionResult<IEnumerable<object>>> GetLastLocations(int page = 1, int pageSize = 10)
        {
            var query = _context.Trajectories
                .GroupBy(t => t.taxi_id)
                .Select(g => g.OrderByDescending(t => t.date).FirstOrDefault());

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var lastLocations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var lastLocationsWithPlate = lastLocations.Select(t => new
            {
                id_trajectorie = t.idtrajectorie,
                taxi_id = t.taxi_id,
                latitude = t.latitude,
                longitude = t.longitude,
                plate = _context.Taxis.FirstOrDefault(tx => tx.idtaxi == t.taxi_id)?.plate,
                date = t.date
            });

            var paginationMetadata = new
            {
                total = totalCount,
                totalPages,
                currentPage = page,
                nextPage = page < totalPages ? page + 1 : (int?)null,
                lastPage = totalPages,
                data = lastLocationsWithPlate
            };

            return Ok(paginationMetadata);
        }


    }
}
