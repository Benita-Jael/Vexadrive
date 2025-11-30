using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VexaDriveAPI.Context;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly Services.Lifecycle.IServiceLifecycle _lifecycle;
        private readonly Repository.ServiceRequestServices.IServiceRequestRepository _serviceRepo;
        private readonly Repository.BillServices.IBillRepository _billRepo;
        private readonly Repository.NotificationServices.INotificationRepository _notificationRepo;

        public AdminController(UserManager<IdentityUser> userManager, IWebHostEnvironment env, Services.Lifecycle.IServiceLifecycle lifecycle,
            Repository.ServiceRequestServices.IServiceRequestRepository serviceRepo,
            Repository.BillServices.IBillRepository billRepo,
            Repository.NotificationServices.INotificationRepository notificationRepo)
        {
            _userManager = userManager;
            _env = env;
            _lifecycle = lifecycle;
            _serviceRepo = serviceRepo;
            _billRepo = billRepo;
            _notificationRepo = notificationRepo;
        }

        // GET: api/admin/requests
        [HttpGet("requests")]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _serviceRepo.GetAllRequestsAsync();

            // Enrich each request DTO with owner info (name, email, contact) retrieved from Identity
            var enriched = new List<object>();
            foreach (var r in requests)
            {
                var user = await _userManager.FindByIdAsync(r.CustomerUserId);
                enriched.Add(new
                {
                    r.ServiceRequestId,
                    r.VehicleModel,
                    r.VehicleNumberPlate,
                    r.VehicleType,
                    r.Status,
                    // include bill and notifications so the admin UI can show uploaded bills and related messages
                    Bill = r.Bill,
                    Notifications = r.Notifications,
                    r.ServiceDate,
                    r.CreatedAt,
                    r.EstimatedDeliveryDate,
                    r.ProblemDescription,
                    r.ServiceAddress,
                    OwnerName = user?.UserName ?? string.Empty,
                    OwnerEmail = user?.Email ?? string.Empty,
                    OwnerContact = user?.PhoneNumber ?? string.Empty
                });
            }

            return Ok(enriched);
        }

        // GET: api/admin/requests/search
        [HttpGet("requests/search")]
        public async Task<IActionResult> SearchRequests([FromQuery] string? vehicleType, [FromQuery] string? plateNumber, [FromQuery] string? customerEmail)
        {
            var context = HttpContext.RequestServices.GetService<VexaDriveDbContext>();
            if (context == null) return StatusCode(500, new { Message = "Database context not found." });

            var query = context.ServiceRequests
                .Include(r => r.Vehicle)
                .AsQueryable();

            // Filter by vehicle type
            if (!string.IsNullOrEmpty(vehicleType))
            {
                query = query.Where(r => r.Vehicle!.Type.ToLower().Contains(vehicleType.ToLower()));
            }

            // Filter by plate number
            if (!string.IsNullOrEmpty(plateNumber))
            {
                query = query.Where(r => r.Vehicle!.NumberPlate.ToLower().Contains(plateNumber.ToLower()));
            }

            // Filter by customer email
            if (!string.IsNullOrEmpty(customerEmail))
            {
                var customerIds = await _userManager.Users
                    .Where(u => u.Email!.ToLower().Contains(customerEmail.ToLower()))
                    .Select(u => u.Id)
                    .ToListAsync();
                query = query.Where(r => customerIds.Contains(r.CustomerUserId));
            }

            var results = await query.ToListAsync();

            var enriched = new List<object>();
            foreach (var r in results)
            {
                var user = await _userManager.FindByIdAsync(r.CustomerUserId);
                enriched.Add(new
                {
                    r.ServiceRequestId,
                    r.ProblemDescription,
                    r.ServiceAddress,
                    r.ServiceDate,
                    r.Status,
                    r.CreatedAt,
                    r.EstimatedDeliveryDate,
                    VehicleModel = r.Vehicle!.Model,
                    VehicleNumberPlate = r.Vehicle!.NumberPlate,
                    VehicleType = r.Vehicle!.Type,
                    CustomerUserId = r.CustomerUserId,
                    OwnerName = user?.UserName ?? string.Empty,
                    OwnerEmail = user?.Email ?? string.Empty,
                    OwnerContact = user?.PhoneNumber ?? string.Empty
                });
            }

            return Ok(enriched);
        }

        // PUT: api/admin/requests/{id}/status
        [HttpPut("requests/{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ServiceStatus status)
        {
            var currentDetails = await _serviceRepo.GetRequestByIdAsync(id);
            if (currentDetails == null) return NotFound();

            var current = Enum.Parse<ServiceStatus>(currentDetails.Status);
            if (!_lifecycle.CanTransition(current, status))
            {
                var allowed = _lifecycle.GetAllowedNext(current).Select(s => s.ToString());
                return BadRequest(new { Message = "Invalid status transition.", Current = current.ToString(), AllowedNext = allowed });
            }

            var dto = new DTO.ServiceRequest.ServiceRequestUpdateDTO
            {
                ServiceRequestId = id,
                Status = status
            };

            var updated = await _serviceRepo.UpdateRequestAsync(dto);
            if (updated == null) return StatusCode(500, new { Message = "Failed to update status." });

            return Ok(updated);
        }

        // PUT: api/admin/requests/{id}/etd
        [HttpPut("requests/{id}/etd")]
        public async Task<IActionResult> UpdateETD(int id, [FromBody] DateTime etd)
        {
            var updated = await _serviceRepo.UpdateEtdAsync(id, etd);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // POST: api/admin/requests/{id}/bill
        [HttpPost("requests/{id}/bill")]
        public async Task<IActionResult> UploadBill(int id, IFormFile file)
        {
            var requestDetails = await _serviceRepo.GetRequestByIdAsync(id);
            if (requestDetails == null) return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "No file uploaded or file is empty." });

            var storageFolder = Path.Combine(_env.ContentRootPath, "Storage", "Bills");
            Directory.CreateDirectory(storageFolder);
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(storageFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var bill = await _billRepo.UploadBillAsync(id, file.FileName, file.ContentType ?? "application/octet-stream", filePath);

            // Notify customer that bill is uploaded
            await _notificationRepo.CreateNotificationAsync(requestDetails.CustomerUserId, id, "Bill Uploaded", "Bill uploaded successfully");

            var billDto = await _billRepo.GetBillByRequestIdAsync(id);
            if (billDto == null) return StatusCode(500, "Uploading bill failed. Try again.");

            return Ok(new { message = "Bill uploaded successfully", bill = billDto });
        }

        // DELETE: api/admin/requests/{id}
        [HttpDelete("requests/{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var deleted = await _serviceRepo.DeleteRequestAsync(id);
            if (!deleted) return NotFound(new { Message = "Service request not found." });
            return Ok(new { Message = "Service request deleted successfully." });
        }

        // GET: api/admin/customers
        [HttpGet("customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _userManager.GetUsersInRoleAsync("Customer");
            return Ok(customers.Select(c => new { c.Id, c.Email, c.UserName }));
        }

        // GET: api/admin/notifications
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotificationsForAdmin()
        {
            var adminId = _userManager.GetUserId(User);
            if (adminId == null) return Unauthorized();

            var notifications = await _notificationRepo.GetNotificationsByUserAsync(adminId);
            return Ok(notifications);
        }

        // PUT: api/admin/notifications/{id}/read
        [HttpPut("notifications/{id}/read")]
        public async Task<IActionResult> MarkNotificationAsReadAdmin(int id)
        {
            var adminId = _userManager.GetUserId(User);
            if (adminId == null) return Unauthorized();

            var notif = await _notificationRepo.GetNotificationByIdAsync(id);
            if (notif == null) return NotFound();
            if (notif.UserId != adminId) return Forbid();

            var ok = await _notificationRepo.MarkAsReadAsync(id);
            if (!ok) return StatusCode(500, new { Message = "Failed to mark notification as read." });
            return Ok(new { Message = "Marked as read." });
        }

        // PUT: api/admin/notifications/{id}/unread
        [HttpPut("notifications/{id}/unread")]
        public async Task<IActionResult> MarkNotificationAsUnreadAdmin(int id)
        {
            var adminId = _userManager.GetUserId(User);
            if (adminId == null) return Unauthorized();

            var notif = await _notificationRepo.GetNotificationByIdAsync(id);
            if (notif == null) return NotFound();
            if (notif.UserId != adminId) return Forbid();

            var ok = await _notificationRepo.MarkAsUnreadAsync(id);
            if (!ok) return StatusCode(500, new { Message = "Failed to mark notification as unread." });
            return Ok(new { Message = "Marked as unread." });
        }
    }
}
