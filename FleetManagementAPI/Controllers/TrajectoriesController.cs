using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FleetManagementAPI.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading.Tasks;

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

        /// <summary>
        /// Search trajectories for a given taxi and date.
        /// </summary>
        /// <param name="idtaxi">ID of the taxi.</param>
        /// <param name="date">Date to search trajectories for.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>A paginated list of trajectories for the given taxi and date.</returns>
        /// <response code="200">Returns the paginated list of trajectories.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpPost(Name = "SearchTrajectories")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationMetadata))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> SearchTrajectories(int idtaxi, DateTime date, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Extract the date part from the DateTime object
                var searchDate = date.Date;

                // Query the database for trajectories matching the given idtaxi and date
                var query = _context.Trajectories
                    .Where(t => t.taxi_id == idtaxi &&
                                t.date.Date == searchDate)
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

                var paginationMetadata = new PaginationMetadata
                {
                    total = totalCount,
                    totalPages = totalPages,
                    currentPage = page,
                    nextPage = page < totalPages ? page + 1 : (int?)null,
                    lastPage = totalPages,
                    data = trajectories
                };

                return Ok(paginationMetadata);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get the last recorded locations of all taxis.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>A paginated list of last recorded locations of all taxis.</returns>
        /// <response code="200">Returns the paginated list of last recorded locations.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpGet("/api/last-locations")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationMetadata))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<object>>> GetLastLocations(int page = 1, int pageSize = 10)
        {
            try
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

                var paginationMetadata = new PaginationMetadata
                {
                    total = totalCount,
                    totalPages = totalPages,
                    currentPage = page,
                    nextPage = page < totalPages ? page + 1 : (int?)null,
                    lastPage = totalPages,
                    data = lastLocationsWithPlate
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
