using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VexaDriveAPI.DTO.OwnerDTO;
using VexaDriveAPI.Repository.OwnerServices;
 
namespace VexaDriveAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerRepository _repository;
        private readonly IMapper _mapper;
 
        public OwnerController(IOwnerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
 
        [HttpPost("addOwner")]
        public async Task<IActionResult> CreateOwnerAsync(OwnerCreateDTO ownerCreateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
 
            var owner = await _repository.CreateOwnerAsync(ownerCreateDTO);
            if (owner == null) return StatusCode(500, "Owner creation failed.");
 
            var result = _mapper.Map<OwnerDetailsDTO>(owner);
            return Ok(result);
        }
 
        [HttpPut("updateOwner/{id}")]
        public async Task<IActionResult> UpdateOwnerAsync(int id, OwnerUpdateDTO updateDTO)
        {
            if (id != updateDTO.OwnerId)
                return BadRequest($"Owner ID mismatch: URL ID ({id}) does not match body ID ({updateDTO.OwnerId})");
 
            var updated = await _repository.UpdateOwnerAsync(updateDTO);
            if (!updated) return NotFound("Owner not found.");
 
            return Ok("Owner updated successfully.");
        }
 

        [HttpGet("searchOwner")]
public async Task<ActionResult<IEnumerable<OwnerListDTO>>> SearchOwners(
    int? id,
    string? firstName,
    string? lastName,
    string? email,
    string? contact)
{
    var owners = await _repository.SearchOwnersAsync(id, firstName, lastName, email, contact);
    return Ok(owners);
}

        [HttpDelete("deleteOwner/{id}")]
        public async Task<IActionResult> DeleteOwnerAsync(int id)
        {
            var deleted = await _repository.DeleteOwnerAsync(id);
            if (!deleted) return NotFound("Owner not found.");
 
            return Ok("Owner deleted successfully.");
        }
 
        [HttpGet("AllOwners")]
        public async Task<ActionResult<IEnumerable<OwnerListDTO>>> GetAllOwnersAsync()
        {
            var owners = await _repository.GetAllOwnersAsync();
            return Ok(owners);
        }
 
        [HttpGet("getOwnerById/{id}")]
        public async Task<ActionResult<OwnerDetailsDTO>> GetOwnerByIdAsync(int id)
        {
            var owner = await _repository.GetOwnerByIdAsync(id);
            if (owner == null) return NotFound($"Owner with ID {id} not found.");
 
            return Ok(owner);
        }
    }
}