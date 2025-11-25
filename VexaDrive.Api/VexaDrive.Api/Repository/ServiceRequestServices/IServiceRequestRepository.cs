using VexaDriveAPI.DTO.ServiceRequest;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Repository.ServiceRequestServices
{
    public interface IServiceRequestRepository
    {
        Task<ServiceRequest?> CreateRequestAsync(ServiceRequestCreateDTO dto, string customerUserId);
        Task<bool> UpdateRequestAsync(ServiceRequestUpdateDTO dto);
        Task<ServiceRequestDetailsDTO?> GetRequestByIdAsync(int requestId);
        Task<IEnumerable<ServiceRequestListDTO>> GetRequestsByCustomerAsync(string customerUserId);
        Task<IEnumerable<ServiceRequestListDTO>> GetAllRequestsAsync(); // Admin only
    }
}
