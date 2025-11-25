using VexaDriveAPI.DTO.VehicleDTO;
using VexaDriveAPI.Models;
 
namespace VexaDriveAPI.Repository.VehicleServices
{
    public interface IVehicleRepository
    {
        Task<Vehicle?> CreateVehicleAsync(VehicleCreateDTO vehicleCreateDTO);
        Task<IEnumerable<VehicleListDTO>> GetAllVehiclesAsync();
        Task<VehicleDetailsDTO?> GetVehicleByIdAsync(int vehicleId);
        Task<IEnumerable<VehicleListDTO>> GetVehiclesByOwnerIdAsync(int ownerId);
        Task<bool> UpdateVehicleAsync(VehicleUpdateDTO vehicleUpdateDTO);
        Task<bool> DeleteVehicleAsync(int vehicleId);
        Task<IEnumerable<VehicleListDTO>> SearchVehiclesAsync(
    int? id,
    string? model,
    string? numberPlate,
    string? type,
    string? color,
    int? ownerId);

    }
}