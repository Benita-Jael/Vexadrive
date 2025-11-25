using AutoMapper;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using VexaDriveAPI.DTO.VehicleDTO;

using VexaDriveAPI.Repository.VehicleServices;
 
namespace VexaDriveAPI.Controllers

{

    [Authorize]

    [ApiController]

    [Route("api/[controller]")]

    public class VehicleController : ControllerBase

    {

        private readonly IVehicleRepository _repository;

        private readonly IMapper _mapper;
 
        public VehicleController(IVehicleRepository repository, IMapper mapper)

        {

            _repository = repository;

            _mapper = mapper;

        }
 
        [HttpPost("addVehicle")]

        public async Task<IActionResult> CreateVehicleAsync(VehicleCreateDTO vehicleCreateDTO)

        {

            if (!ModelState.IsValid)

                return BadRequest(ModelState);
 
            var vehicle = await _repository.CreateVehicleAsync(vehicleCreateDTO);

            if (vehicle == null) return StatusCode(500, "Vehicle creation failed.");
 
            var result = _mapper.Map<VehicleDetailsDTO>(vehicle);

            return Ok(result);

        }
 
        [HttpPut("updateVehicle/{id}")]

        public async Task<IActionResult> UpdateVehicleAsync(int id, VehicleUpdateDTO updateDTO)

        {

            if (id != updateDTO.VehicleId)

                return BadRequest($"Vehicle ID mismatch: URL ID ({id}) does not match body ID ({updateDTO.VehicleId})");
 
            var updated = await _repository.UpdateVehicleAsync(updateDTO);

            if (!updated) return NotFound("Vehicle not found.");
 
            return Ok("Vehicle updated successfully.");

        }

        [HttpGet("searchVehicle")]
public async Task<ActionResult<IEnumerable<VehicleListDTO>>> SearchVehicles(
    int? id,
    string? model,
    string? numberPlate,
    string? type,
    string? color,
    int? ownerId)
{
    var vehicles = await _repository.SearchVehiclesAsync(id, model, numberPlate, type, color, ownerId);
    return Ok(vehicles);
}

 
        [HttpDelete("deleteVehicle/{id}")]

        public async Task<IActionResult> DeleteVehicleAsync(int id)

        {

            var deleted = await _repository.DeleteVehicleAsync(id);

            if (!deleted) return NotFound("Vehicle not found.");
 
            return Ok("Vehicle deleted successfully.");

        }
 
        [HttpGet("AllVehicles")]

        public async Task<ActionResult<IEnumerable<VehicleListDTO>>> GetAllVehiclesAsync()

        {

            var vehicles = await _repository.GetAllVehiclesAsync();

            return Ok(vehicles);

        }
 
        [HttpGet("getVehicleById/{id}")]

        public async Task<ActionResult<VehicleDetailsDTO>> GetVehicleByIdAsync(int id)

        {

            var vehicle = await _repository.GetVehicleByIdAsync(id);

            if (vehicle == null) return NotFound($"Vehicle with ID {id} not found.");
 
            return Ok(vehicle);

        }
 
        [HttpGet("getVehiclesByOwner/{ownerId}")]

        public async Task<ActionResult<IEnumerable<VehicleListDTO>>> GetVehiclesByOwnerAsync(int ownerId)

        {

            var vehicles = await _repository.GetVehiclesByOwnerIdAsync(ownerId);

            if (vehicles == null || !vehicles.Any())

                return NotFound($"No vehicles found for Owner ID {ownerId}.");
 
            return Ok(vehicles);

        }

    }

}
