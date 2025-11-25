using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using VexaDriveAPI.Context;
using VexaDriveAPI.DTO.OwnerDTO;
using VexaDriveAPI.Models;
 
namespace VexaDriveAPI.Repository.OwnerServices
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly VexaDriveDbContext _context;
        private readonly IMapper _mapper;
 
        public OwnerRepository(VexaDriveDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
 
        public async Task<Owner?> CreateOwnerAsync(OwnerCreateDTO ownerCreateDTO)
        {
            var existingOwner = await _context.Owners.FirstOrDefaultAsync(o =>
                o.FirstName == ownerCreateDTO.FirstName &&
                o.LastName == ownerCreateDTO.LastName &&
                o.ContactNumber == ownerCreateDTO.ContactNumber);
 
            if (existingOwner != null)
                throw new Exception("Owner with the same name and contact number already exists.");
 
            var owner = _mapper.Map<Owner>(ownerCreateDTO);
 
            await _context.Owners.AddAsync(owner);
            await _context.SaveChangesAsync();
 
            return owner;
        }
 
        public async Task<IEnumerable<OwnerListDTO>> GetAllOwnersAsync()
        {
            var owners = await _context.Owners.ToListAsync();
            return _mapper.Map<List<OwnerListDTO>>(owners);
        }
 
        public async Task<OwnerDetailsDTO?> GetOwnerByIdAsync(int ownerId)
        {
            var owner = await _context.Owners.Include(o => o.Vehicles)
                                             .FirstOrDefaultAsync(o => o.OwnerId == ownerId);
            if (owner == null) return null;
            return _mapper.Map<OwnerDetailsDTO>(owner);
        }
 
        public async Task<IEnumerable<OwnerListDTO>> SearchOwnersAsync(
    int? id,
    string? firstName,
    string? lastName,
    string? email,
    string? contact)
{
    var query = _context.Owners.AsQueryable();

    if (id.HasValue)
        query = query.Where(o => o.OwnerId == id.Value);

    if (!string.IsNullOrEmpty(firstName))
        query = query.Where(o => o.FirstName.Contains(firstName));

    if (!string.IsNullOrEmpty(lastName))
        query = query.Where(o => o.LastName.Contains(lastName));

    if (!string.IsNullOrEmpty(email))
        query = query.Where(o => o.Email.Contains(email));

    if (!string.IsNullOrEmpty(contact))
    query = query.Where(o => o.ContactNumber.ToLower().Contains(contact.ToLower()));

    var owners = await query.ToListAsync();
    return _mapper.Map<List<OwnerListDTO>>(owners);
}

        public async Task<bool> UpdateOwnerAsync(OwnerUpdateDTO ownerUpdateDTO)
        {
            var owner = await _context.Owners.FindAsync(ownerUpdateDTO.OwnerId);
            if (owner == null) return false;
 
            _mapper.Map(ownerUpdateDTO, owner);
            await _context.SaveChangesAsync();
 
            return true;
        }
 
        public async Task<bool> DeleteOwnerAsync(int ownerId)
        {
            var owner = await _context.Owners.FindAsync(ownerId);
            if (owner == null) return false;
 
            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();
 
            return true;
        }
    }
}