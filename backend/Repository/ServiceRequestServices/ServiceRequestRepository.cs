using AutoMapper;
using System.IO;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;

        public ServiceRequestRepository(VexaDriveDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
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

            // Create initial notification for the customer
            var notification = new Models.Notification
            {
                ServiceRequestId = request.ServiceRequestId,
                UserId = customerUserId,
                Title = "Service Request Created",
                Message = $"Your request for vehicle {vehicle.Model} ({vehicle.NumberPlate}) has been created.",
                CreatedAt = DateTime.UtcNow
            };
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            // Also: create a notification for the customer for bill upload, status update, and ETD update (if not already present in those methods)

            // ALSO: notify all Admin users that a new request was raised, including owner name and email in the message/title
            try
            {
                var owner = await _userManager.FindByIdAsync(customerUserId);
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                foreach (var admin in admins)
                {
                    var adminNotif = new Models.Notification
                    {
                        ServiceRequestId = request.ServiceRequestId,
                        UserId = admin.Id,
                        Title = $"{owner?.UserName ?? "Unknown"} ({owner?.Email ?? ""}) raised a new request",
                        Message = $"Request {request.ServiceRequestId} for vehicle {vehicle.Model} ({vehicle.NumberPlate}) was raised by {owner?.UserName ?? "Unknown"} ({owner?.Email ?? ""})",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.Notifications.AddAsync(adminNotif);
                }
                await _context.SaveChangesAsync();
            }
            catch
            {
                // swallow errors so request creation still succeeds even if admin notification fails
            }

            return request;
        }

        public async Task<ServiceRequestDetailsDTO?> UpdateRequestAsync(ServiceRequestUpdateDTO dto)
        {
            var request = await _context.ServiceRequests
                .Include(r => r.Vehicle)
                .Include(r => r.Bill)
                .Include(r => r.Notifications)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == dto.ServiceRequestId);

            if (request == null) return null;

            var previousStatus = request.Status;
            request.Status = dto.Status;
            request.EstimatedDeliveryDate = dto.EstimatedDeliveryDate;
            request.UpdatedAt = DateTime.UtcNow;

            // If status changed, create a notification for the customer
            if (previousStatus != dto.Status)
            {
                var notification = new Models.Notification
                {
                    ServiceRequestId = request.ServiceRequestId,
                    UserId = request.CustomerUserId,
                    Title = "Service Status Updated",
                    Message = $"Your request status is now {dto.Status}",
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Notifications.AddAsync(notification);
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<ServiceRequestDetailsDTO>(request);
        }

        public async Task<ServiceRequestDetailsDTO?> UpdateEtdAsync(int requestId, DateTime etd)
        {
            var request = await _context.ServiceRequests
                .Include(r => r.Vehicle)
                .Include(r => r.Bill)
                .Include(r => r.Notifications)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == requestId);

            if (request == null) return null;

            request.EstimatedDeliveryDate = etd;
            request.UpdatedAt = DateTime.UtcNow;

            // Create notification for the customer
            var notification = new Models.Notification
            {
                ServiceRequestId = request.ServiceRequestId,
                UserId = request.CustomerUserId,
                Title = "Estimated Delivery Updated",
                Message = $"Your vehicle will be ready by {etd:yyyy-MM-dd}",
                CreatedAt = DateTime.UtcNow
            };
            await _context.Notifications.AddAsync(notification);

            await _context.SaveChangesAsync();

            return _mapper.Map<ServiceRequestDetailsDTO>(request);
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

        public async Task<IEnumerable<ServiceRequestDetailsDTO>> GetRequestsByCustomerAsync(string customerUserId)
        {
            var requests = await _context.ServiceRequests
                .Include(r => r.Vehicle)
                .Include(r => r.Bill)
                .Include(r => r.Notifications)
                .Where(r => r.CustomerUserId == customerUserId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<ServiceRequestDetailsDTO>>(requests);
        }

        public async Task<IEnumerable<ServiceRequestDetailsDTO>> GetAllRequestsAsync()
        {
            var requests = await _context.ServiceRequests
                .Include(r => r.Vehicle)
                .Include(r => r.Bill)
                .Include(r => r.Notifications)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<ServiceRequestDetailsDTO>>(requests);
        }

        public async Task<bool> DeleteRequestAsync(int requestId)
        {
            var request = await _context.ServiceRequests
                .Include(r => r.Bill)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == requestId);

            if (request == null) return false;

            // If there's an associated bill with a stored file, attempt to delete the file from disk
            try
            {
                if (request.Bill != null && !string.IsNullOrEmpty(request.Bill.StoragePath) && File.Exists(request.Bill.StoragePath))
                {
                    File.Delete(request.Bill.StoragePath);
                }
            }
            catch
            {
                // swallow file deletion errors - deletion should continue for DB consistency
            }

            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
