using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VexaDriveAPI.Context;
using VexaDriveAPI.DTO.VehicleDTO;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly VexaDriveDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public VehicleController(VexaDriveDbContext context, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // POST: api/vehicle/add
        [Authorize(Roles = "Customer")]
        [HttpPost("add")]
        public async Task<IActionResult> AddVehicle([FromBody] VehicleCreateDTO dto)
        {

            try
            {
                // Debug: log incoming principal claims to help diagnose auth/role issues
                try
                {
                    Console.WriteLine("[VehicleController] Incoming claims:");
                    foreach (var c in User.Claims)
                    {
                        Console.WriteLine($"[VehicleController] Claim: {c.Type} = {c.Value}");
                    }
                    Console.WriteLine($"[VehicleController] IsAuthenticated: {User.Identity?.IsAuthenticated}");
                    Console.WriteLine($"[VehicleController] IsInRole Customer: {User.IsInRole("Customer")}");
                }
                catch { }
                var userId = _userManager.GetUserId(User);
                if (userId == null) return Unauthorized();

                var vehicle = new Vehicle
                {
                    Model = dto.Model,
                    NumberPlate = dto.NumberPlate,
                    Type = dto.Type,
                    Color = dto.Color,
                    CustomerUserId = userId
                };

                await _context.Vehicles.AddAsync(vehicle);
                await _context.SaveChangesAsync();

                // Return exact message expected by frontend
                return Ok("Vehicle added successfully");
            }
            catch (Exception ex)
            {
                // Return a simple failure message so frontend can display the required text
                return StatusCode(500, "Vehicle creation failed");
            }
        }

        // PUT: api/vehicle/update/{id}
        [Authorize(Roles = "Customer")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] VehicleUpdateDTO dto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == id && v.CustomerUserId == userId);
            if (vehicle == null) return NotFound();

            vehicle.Model = dto.Model;
            vehicle.NumberPlate = dto.NumberPlate;
            vehicle.Type = dto.Type;
            vehicle.Color = dto.Color;

            await _context.SaveChangesAsync();
            return Ok("Vehicle updated successfully.");
        }

        // DELETE: api/vehicle/delete/{id}
        [Authorize(Roles = "Customer")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == id && v.CustomerUserId == userId);
            if (vehicle == null) return NotFound();

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return Ok("Vehicle deleted successfully.");
        }

        // GET: api/vehicle/all (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllVehicles()
        {
            var vehicles = await _context.Vehicles.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<VehicleListDTO>>(vehicles));
        }

        // GET: api/vehicle/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return NotFound();

            return Ok(_mapper.Map<VehicleDetailsDTO>(vehicle));
        }

        // GET: api/vehicle/search (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchVehicles(string? model, string? numberPlate, string? type, string? color, string? customerUserId)
        {
            var query = _context.Vehicles.AsQueryable();

            if (!string.IsNullOrEmpty(model)) query = query.Where(v => v.Model.Contains(model));
            if (!string.IsNullOrEmpty(numberPlate)) query = query.Where(v => v.NumberPlate.Contains(numberPlate));
            if (!string.IsNullOrEmpty(type)) query = query.Where(v => v.Type.Contains(type));
            if (!string.IsNullOrEmpty(color)) query = query.Where(v => v.Color.Contains(color));
            if (!string.IsNullOrEmpty(customerUserId)) query = query.Where(v => v.CustomerUserId == customerUserId);

            var vehicles = await query.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<VehicleListDTO>>(vehicles));
        }
    }
}
