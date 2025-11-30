using VexaDriveAPI.DTO.ServiceRequest;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Repository.ServiceRequestServices
{
    public interface IServiceRequestRepository
    {
        Task<ServiceRequest?> CreateRequestAsync(ServiceRequestCreateDTO dto, string customerUserId);
    Task<DTO.ServiceRequest.ServiceRequestDetailsDTO?> UpdateRequestAsync(DTO.ServiceRequest.ServiceRequestUpdateDTO dto);
    Task<DTO.ServiceRequest.ServiceRequestDetailsDTO?> UpdateEtdAsync(int requestId, DateTime etd);
    Task<ServiceRequestDetailsDTO?> GetRequestByIdAsync(int requestId);
    // Return detailed DTOs for customer lists so the client can see bill/notification info
    Task<IEnumerable<DTO.ServiceRequest.ServiceRequestDetailsDTO>> GetRequestsByCustomerAsync(string customerUserId);
    // Admin list should also return detailed DTOs (includes bill & notifications)
    Task<IEnumerable<DTO.ServiceRequest.ServiceRequestDetailsDTO>> GetAllRequestsAsync(); // Admin only
    // Delete a service request (admin) - returns true if deleted, false if not found
    Task<bool> DeleteRequestAsync(int requestId);
    }
}
