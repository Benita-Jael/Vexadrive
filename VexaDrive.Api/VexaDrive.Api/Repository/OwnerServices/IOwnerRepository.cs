using VexaDriveAPI.DTO.OwnerDTO;
using VexaDriveAPI.Models;
 
namespace VexaDriveAPI.Repository.OwnerServices
{
    public interface IOwnerRepository
    {
        Task<Owner?> CreateOwnerAsync(OwnerCreateDTO ownerCreateDTO);
        Task<IEnumerable<OwnerListDTO>> GetAllOwnersAsync();
        Task<OwnerDetailsDTO?> GetOwnerByIdAsync(int ownerId);
        Task<bool> UpdateOwnerAsync(OwnerUpdateDTO ownerUpdateDTO);
        Task<bool> DeleteOwnerAsync(int ownerId);
        Task<IEnumerable<OwnerListDTO>> SearchOwnersAsync(
    int? id,
    string? firstName,
    string? lastName,
    string? email,
    string? contact);


    }
}