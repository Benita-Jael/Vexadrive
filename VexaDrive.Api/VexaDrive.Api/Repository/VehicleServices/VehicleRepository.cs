using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using VexaDriveAPI.Context;
using VexaDriveAPI.DTO.VehicleDTO;
using VexaDriveAPI.Models;
 
namespace VexaDriveAPI.Repository.VehicleServices
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly VexaDriveDbContext _context;
        private readonly IMapper _mapper;
 
        public VehicleRepository(VexaDriveDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
 
        public async Task<Vehicle?> CreateVehicleAsync(VehicleCreateDTO vehicleCreateDTO)
        {
            var existingVehicle = await _context.Vehicles.FirstOrDefaultAsync(v =>
                v.NumberPlate == vehicleCreateDTO.NumberPlate);
 
            if (existingVehicle != null)
                throw new Exception("Vehicle with the same number plate already exists.");
 
            var owner = await _context.Owners.FindAsync(vehicleCreateDTO.OwnerId);
            if (owner == null)
                throw new ValidationException("Invalid OwnerId. No such owner exists.");
 
            var vehicle = _mapper.Map<Vehicle>(vehicleCreateDTO);
            vehicle.Owner = owner;
 
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
 
            return vehicle;
        }
 
        public async Task<IEnumerable<VehicleListDTO>> GetAllVehiclesAsync()
        {
            var vehicles = await _context.Vehicles.Include(v => v.Owner).ToListAsync();
            return _mapper.Map<List<VehicleListDTO>>(vehicles);
        }
 

        public async Task<IEnumerable<VehicleListDTO>> SearchVehiclesAsync(
    int? id,
    string? model,
    string? numberPlate,
    string? type,
    string? color,
    int? ownerId)
{
    var query = _context.Vehicles.Include(v => v.Owner).AsQueryable();

    if (id.HasValue)
        query = query.Where(v => v.VehicleId == id.Value);

    if (!string.IsNullOrEmpty(model))
        query = query.Where(v => v.Model.Contains(model));

    if (!string.IsNullOrEmpty(numberPlate))
        query = query.Where(v => v.NumberPlate.Contains(numberPlate));

    if (!string.IsNullOrEmpty(type))
        query = query.Where(v => v.Type.Contains(type));

    if (!string.IsNullOrEmpty(color))
        query = query.Where(v => v.Color.Contains(color));

    if (ownerId.HasValue)
        query = query.Where(v => v.OwnerId == ownerId.Value);

    var vehicles = await query.ToListAsync();
    return _mapper.Map<List<VehicleListDTO>>(vehicles);
}

        public async Task<VehicleDetailsDTO?> GetVehicleByIdAsync(int vehicleId)
        {
            var vehicle = await _context.Vehicles.Include(v => v.Owner)
                                                 .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
            if (vehicle == null) return null;
            return _mapper.Map<VehicleDetailsDTO>(vehicle);
        }
 
        public async Task<IEnumerable<VehicleListDTO>> GetVehiclesByOwnerIdAsync(int ownerId)
        {
            var vehicles = await _context.Vehicles
                                         .Where(v => v.OwnerId == ownerId)
                                         .Include(v => v.Owner)
                                         .ToListAsync();
            return _mapper.Map<List<VehicleListDTO>>(vehicles);
        }
 
        public async Task<bool> UpdateVehicleAsync(VehicleUpdateDTO vehicleUpdateDTO)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleUpdateDTO.VehicleId);
            if (vehicle == null) return false;
 
            if (vehicleUpdateDTO.OwnerId.HasValue)
            {
                var owner = await _context.Owners.FindAsync(vehicleUpdateDTO.OwnerId.Value);
                if (owner == null)
                    throw new ValidationException("Invalid OwnerId. No such owner exists.");
                vehicle.Owner = owner;
            }
 
            _mapper.Map(vehicleUpdateDTO, vehicle);
            await _context.SaveChangesAsync();
 
            return true;
        }
 
        public async Task<bool> DeleteVehicleAsync(int vehicleId)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null) return false;
 
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
 
            return true;
        }
    }
}