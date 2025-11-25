using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VexaDriveAPI.Context;
using VexaDriveAPI.DTO.ServiceRequest;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Repository.ServiceRequestServices
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly VexaDriveDbContext _context;
        private readonly IMapper _mapper;

        public ServiceRequestRepository(VexaDriveDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceRequest?> CreateRequestAsync(ServiceRequestCreateDTO dto, string customerUserId)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == dto.VehicleId && v.CustomerUserId == customerUserId);
            if (vehicle == null)
                throw new Exception("Invalid Vehicle. Customers can only raise requests for their own vehicles.");

            var request = _mapper.Map<ServiceRequest>(dto);
            request.CustomerUserId = customerUserId;
            request.Status = ServiceStatus.RequestCreated;
            request.CreatedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.ServiceRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<bool> UpdateRequestAsync(ServiceRequestUpdateDTO dto)
        {
            var request = await _context.ServiceRequests.FindAsync(dto.ServiceRequestId);
            if (request == null) return false;

            request.Status = dto.Status;
            request.EstimatedDeliveryDate = dto.EstimatedDeliveryDate;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ServiceRequestDetailsDTO?> GetRequestByIdAsync(int requestId)
        {
            var request = await _context.ServiceRequests
                .Include(r => r.Vehicle)
                .Include(r => r.Notifications)
                .Include(r => r.Bill)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == requestId);

            if (request == null) return null;
            return _mapper.Map<ServiceRequestDetailsDTO>(request);
        }

        public async Task<IEnumerable<ServiceRequestListDTO>> GetRequestsByCustomerAsync(string customerUserId)
        {
            var requests = await _context.ServiceRequests
                .Include(r => r.Vehicle)
                .Where(r => r.CustomerUserId == customerUserId)
                .ToListAsync();

            return _mapper.Map<List<ServiceRequestListDTO>>(requests);
        }

        public async Task<IEnumerable<ServiceRequestListDTO>> GetAllRequestsAsync()
        {
            var requests = await _context.ServiceRequests
                .Include(r => r.Vehicle)
                .ToListAsync();

            return _mapper.Map<List<ServiceRequestListDTO>>(requests);
        }
    }
}
