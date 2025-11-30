using VexaDriveAPI.DTO.VehicleDTO;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Repository.VehicleServices
{
    public interface IVehicleRepository
    {
        Task<Vehicle?> CreateVehicleAsync(VehicleCreateDTO vehicleCreateDTO, string customerUserId);
        Task<IEnumerable<VehicleListDTO>> GetAllVehiclesAsync();
        Task<VehicleDetailsDTO?> GetVehicleByIdAsync(int vehicleId);
        Task<IEnumerable<VehicleListDTO>> GetVehiclesByCustomerIdAsync(string customerUserId);
        Task<bool> UpdateVehicleAsync(VehicleUpdateDTO vehicleUpdateDTO, string customerUserId);
        Task<bool> DeleteVehicleAsync(int vehicleId, string customerUserId);
        Task<IEnumerable<VehicleListDTO>> SearchVehiclesAsync(
            int? id,
            string? model,
            string? numberPlate,
            string? type,
            string? color,
            string? customerUserId);
    }
}
