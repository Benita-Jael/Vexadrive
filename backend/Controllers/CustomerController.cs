using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VexaDriveAPI.Context;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Controllers
{
    [ApiController]
    [Route("api/customer")]
    [Authorize(Roles = "Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AutoMapper.IMapper _mapper;
        private readonly Repository.ServiceRequestServices.IServiceRequestRepository _serviceRepo;
        private readonly Repository.NotificationServices.INotificationRepository _notificationRepo;
        private readonly Repository.BillServices.IBillRepository _billRepo;

        public CustomerController(UserManager<IdentityUser> userManager,
            AutoMapper.IMapper mapper,
            Repository.ServiceRequestServices.IServiceRequestRepository serviceRepo,
            Repository.NotificationServices.INotificationRepository notificationRepo,
            Repository.BillServices.IBillRepository billRepo)
        {
            _userManager = userManager;
            _mapper = mapper;
            _serviceRepo = serviceRepo;
            _notificationRepo = notificationRepo;
            _billRepo = billRepo;
        }

        // POST: api/customer/requests
        [HttpPost("requests")]
        public async Task<IActionResult> CreateRequest([FromBody] DTO.ServiceRequest.ServiceRequestCreateDTO requestDto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            ServiceRequest? created;
            try
            {
                created = await _serviceRepo.CreateRequestAsync(requestDto, userId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

            if (created == null) return StatusCode(500, "Request submission failed");

            // Notify all admins of new request using customer's email in the message
            try
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                var customer = await _userManager.FindByIdAsync(userId);
                var customerEmail = customer?.Email ?? "A customer";
                foreach (var admin in admins)
                {
                    await _notificationRepo.CreateNotificationAsync(admin.Id, created.ServiceRequestId, "New Service Request", $"{customerEmail} created a new request");
                }
            }
            catch { /* logging omitted - non-blocking */ }

            var details = await _serviceRepo.GetRequestByIdAsync(created.ServiceRequestId);
            // Return the required message along with details for UI convenience
            return Ok(new { message = "Request submitted successfully", details });
        }

        // GET: api/customer/requests
        [HttpGet("requests")]
        public async Task<IActionResult> GetRequests()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var requests = await _serviceRepo.GetRequestsByCustomerAsync(userId);
            return Ok(requests);
        }

        // GET: api/customer/requests/{id}
        [HttpGet("requests/{id}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var request = await _serviceRepo.GetRequestByIdAsync(id);
            if (request == null) return NotFound();
            if (request.CustomerUserId != userId) return Forbid();

            return Ok(request);
        }

        // GET: api/customer/notifications
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var notifications = await _notificationRepo.GetNotificationsByUserAsync(userId);
            return Ok(notifications);
        }

        // GET: api/customer/vehicles
        [HttpGet("vehicles")]
        public async Task<IActionResult> GetCustomerVehicles()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var context = HttpContext.RequestServices.GetService<VexaDriveDbContext>();
            if (context == null) return StatusCode(500, new { Message = "Database context not found." });

            var vehicles = await context.Vehicles
                .Where(v => v.CustomerUserId == userId)
                .Select(v => new
                {
                    v.VehicleId,
                    v.Model,
                    v.NumberPlate,
                    v.Type,
                    v.Color
                })
                .ToListAsync();

            return Ok(vehicles);
        }

        // PUT: api/customer/notifications/{id}/read
        [HttpPut("notifications/{id}/read")]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var notif = await _notificationRepo.GetNotificationByIdAsync(id);
            if (notif == null || notif.UserId != userId) return NotFound();

            var ok = await _notificationRepo.MarkAsReadAsync(id);
            if (!ok) return StatusCode(500, new { Message = "Failed to mark notification as read." });
            return Ok(new { Message = "Marked as read." });
        }

        // PUT: api/customer/notifications/{id}/unread
        [HttpPut("notifications/{id}/unread")]
        public async Task<IActionResult> MarkNotificationAsUnread(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var notif = await _notificationRepo.GetNotificationByIdAsync(id);
            if (notif == null || notif.UserId != userId) return NotFound();

            var ok = await _notificationRepo.MarkAsUnreadAsync(id);
            if (!ok) return StatusCode(500, new { Message = "Failed to mark notification as unread." });
            return Ok(new { Message = "Marked as unread." });
        }

        // GET: api/customer/requests/{id}/bill
        [HttpGet("requests/{id}/bill")]
        public async Task<IActionResult> DownloadBill(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var request = await _serviceRepo.GetRequestByIdAsync(id);
            if (request == null) return NotFound();
            if (request.CustomerUserId != userId) return Forbid();

            var bill = await _billRepo.GetBillByRequestIdAsync(id);
            if (bill == null) return NotFound();

            if (string.IsNullOrEmpty(bill.StoragePath) || !System.IO.File.Exists(bill.StoragePath))
                return NotFound(new { Message = "Bill file not found on server." });

            var fileBytes = await System.IO.File.ReadAllBytesAsync(bill.StoragePath);
            return File(fileBytes, bill.ContentType, bill.FileName);
        }
    }
}
