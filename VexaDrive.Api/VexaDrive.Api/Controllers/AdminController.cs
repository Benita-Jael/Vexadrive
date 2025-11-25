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
        private readonly VexaDriveDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(VexaDriveDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/admin/requests
        [HttpGet("requests")]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _context.ServiceRequests
                .Include(r => r.Vehicle)
                .Include(r => r.Bill)
                .Include(r => r.Notifications)
                .ToListAsync();

            return Ok(requests);
        }

        // PUT: api/admin/requests/{id}/status
        [HttpPut("requests/{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ServiceStatus status)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = status;
            request.UpdatedAt = DateTime.UtcNow;

            // Notify customer
            var notification = new Notification
            {
                ServiceRequestId = request.ServiceRequestId,
                UserId = request.CustomerUserId,
                Title = "Service Status Updated",
                Message = $"Your request status is now {status}",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
            return Ok(request);
        }

        // PUT: api/admin/requests/{id}/eta
        [HttpPut("requests/{id}/eta")]
        public async Task<IActionResult> UpdateETA(int id, [FromBody] DateTime eta)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.EstimatedDeliveryDate = eta;
            request.UpdatedAt = DateTime.UtcNow;

            // Notify customer
            var notification = new Notification
            {
                ServiceRequestId = request.ServiceRequestId,
                UserId = request.CustomerUserId,
                Title = "Estimated Delivery Updated",
                Message = $"Your vehicle will be ready by {eta}",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
            return Ok(request);
        }

        // POST: api/admin/requests/{id}/bill
        [HttpPost("requests/{id}/bill")]
        public async Task<IActionResult> UploadBill(int id, IFormFile file)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            var filePath = Path.Combine("Storage/Bills", $"{Guid.NewGuid()}_{file.FileName}");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var bill = new Bill
            {
                ServiceRequestId = request.ServiceRequestId,
                FileName = file.FileName,
                ContentType = file.ContentType,
                StoragePath = filePath,
                UploadedAt = DateTime.UtcNow
            };

            _context.Bills.Add(bill);

            // Notify customer
            var notification = new Notification
            {
                ServiceRequestId = request.ServiceRequestId,
                UserId = request.CustomerUserId,
                Title = "Bill Uploaded",
                Message = "Your service bill is now available for download.",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
            return Ok(bill);
        }

        // GET: api/admin/customers
        [HttpGet("customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _userManager.GetUsersInRoleAsync("Customer");
            return Ok(customers.Select(c => new { c.Id, c.Email, c.UserName }));
        }
    }
}
