using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Vehicle?> CreateVehicleAsync(VehicleCreateDTO vehicleCreateDTO, string customerUserId)
        {
            var existingVehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.NumberPlate == vehicleCreateDTO.NumberPlate);

            if (existingVehicle != null)
                throw new Exception("Vehicle with the same number plate already exists.");

            var vehicle = _mapper.Map<Vehicle>(vehicleCreateDTO);
            vehicle.CustomerUserId = customerUserId;

            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();

            return vehicle;
        }

        public async Task<IEnumerable<VehicleListDTO>> GetAllVehiclesAsync()
        {
            var vehicles = await _context.Vehicles.ToListAsync();
            return _mapper.Map<List<VehicleListDTO>>(vehicles);
        }

        public async Task<IEnumerable<VehicleListDTO>> SearchVehiclesAsync(
            int? id,
            string? model,
            string? numberPlate,
            string? type,
            string? color,
            string? customerUserId)
        {
            var query = _context.Vehicles.AsQueryable();

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

            if (!string.IsNullOrEmpty(customerUserId))
                query = query.Where(v => v.CustomerUserId == customerUserId);

            var vehicles = await query.ToListAsync();
            return _mapper.Map<List<VehicleListDTO>>(vehicles);
        }

        public async Task<VehicleDetailsDTO?> GetVehicleByIdAsync(int vehicleId)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

            if (vehicle == null) return null;
            return _mapper.Map<VehicleDetailsDTO>(vehicle);
        }

        public async Task<IEnumerable<VehicleListDTO>> GetVehiclesByCustomerIdAsync(string customerUserId)
        {
            var vehicles = await _context.Vehicles
                .Where(v => v.CustomerUserId == customerUserId)
                .ToListAsync();

            return _mapper.Map<List<VehicleListDTO>>(vehicles);
        }

        public async Task<bool> UpdateVehicleAsync(VehicleUpdateDTO vehicleUpdateDTO, string customerUserId)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleUpdateDTO.VehicleId && v.CustomerUserId == customerUserId);

            if (vehicle == null) return false;

            _mapper.Map(vehicleUpdateDTO, vehicle);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteVehicleAsync(int vehicleId, string customerUserId)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId && v.CustomerUserId == customerUserId);

            if (vehicle == null) return false;

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
