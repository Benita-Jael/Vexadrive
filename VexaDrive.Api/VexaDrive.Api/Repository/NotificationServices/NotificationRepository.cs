using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VexaDriveAPI.Context;
using VexaDriveAPI.DTO.Notification;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Repository.NotificationServices
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly VexaDriveDbContext _context;
        private readonly IMapper _mapper;

        public NotificationRepository(VexaDriveDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Notification?> CreateNotificationAsync(string userId, int serviceRequestId, string title, string message)
        {
            var request = await _context.ServiceRequests.FindAsync(serviceRequestId);
            if (request == null)
                throw new Exception("Invalid ServiceRequestId. Cannot create notification.");

            var notification = new Notification
            {
                UserId = userId,
                ServiceRequestId = serviceRequestId,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            return notification;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NotificationListDTO>> GetNotificationsByUserAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<NotificationListDTO>>(notifications);
        }

        public async Task<NotificationDetailsDTO?> GetNotificationByIdAsync(int notificationId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

            if (notification == null) return null;
            return _mapper.Map<NotificationDetailsDTO>(notification);
        }
    }
}
