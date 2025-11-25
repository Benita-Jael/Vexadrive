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
        private readonly VexaDriveDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomerController(VexaDriveDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: api/customer/requests
        [HttpPost("requests")]
        public async Task<IActionResult> CreateRequest([FromBody] ServiceRequest request)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            request.CustomerUserId = userId;
            request.Status = ServiceStatus.RequestCreated;
            request.CreatedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;

            _context.ServiceRequests.Add(request);

            // Create notification
            var notification = new Notification
            {
                ServiceRequest = request,
                UserId = userId,
                Title = "Service Request Created",
                Message = $"Your request for vehicle {request.VehicleId} has been created.",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
            return Ok(request);
        }

        // GET: api/customer/requests
        [HttpGet("requests")]
        public async Task<IActionResult> GetRequests()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var requests = await _context.ServiceRequests
                .Include(r => r.Vehicle)
                .Include(r => r.Bill)
                .Where(r => r.CustomerUserId == userId)
                .ToListAsync();

            return Ok(requests);
        }

        // GET: api/customer/requests/{id}
        [HttpGet("requests/{id}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var request = await _context.ServiceRequests
                .Include(r => r.Vehicle)
                .Include(r => r.Bill)
                .Include(r => r.Notifications)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == id && r.CustomerUserId == userId);

            if (request == null) return NotFound();

            return Ok(request);
        }

        // GET: api/customer/notifications
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }

        // PUT: api/customer/notifications/{id}/read
        [HttpPut("notifications/{id}/read")]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == userId);

            if (notification == null) return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(notification);
        }

        // GET: api/customer/requests/{id}/bill
        [HttpGet("requests/{id}/bill")]
        public async Task<IActionResult> DownloadBill(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var bill = await _context.Bills
                .Include(b => b.ServiceRequest)
                .FirstOrDefaultAsync(b => b.ServiceRequestId == id && b.ServiceRequest.CustomerUserId == userId);

            if (bill == null) return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(bill.StoragePath);
            return File(fileBytes, bill.ContentType, bill.FileName);
        }
    }
}
