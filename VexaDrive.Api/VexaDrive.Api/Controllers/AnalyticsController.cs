using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VexaDriveAPI.Context;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Controllers
{
    [ApiController]
    [Route("api/admin/analytics")]
    [Authorize(Roles = "Admin")]
    public class AnalyticsController : ControllerBase
    {
        private readonly VexaDriveDbContext _context;

        public AnalyticsController(VexaDriveDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/analytics/status-counts
        [HttpGet("status-counts")]
        public async Task<IActionResult> GetStatusCounts()
        {
            var counts = await _context.ServiceRequests
                .GroupBy(sr => sr.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(counts);
        }

        // GET: api/admin/analytics/vehicle-type-counts
        [HttpGet("vehicle-type-counts")]
        public async Task<IActionResult> GetVehicleTypeCounts()
        {
            var counts = await _context.Vehicles
                .GroupBy(v => v.Type)
                .Select(g => new
                {
                    VehicleType = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(counts);
        }
    }
}
