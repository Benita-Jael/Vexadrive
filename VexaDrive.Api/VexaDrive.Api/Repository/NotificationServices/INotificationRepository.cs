using VexaDriveAPI.DTO.Notification;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Repository.NotificationServices
{
    public interface INotificationRepository
    {
        Task<Notification?> CreateNotificationAsync(string userId, int serviceRequestId, string title, string message);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<IEnumerable<NotificationListDTO>> GetNotificationsByUserAsync(string userId);
        Task<NotificationDetailsDTO?> GetNotificationByIdAsync(int notificationId);
    }
}
