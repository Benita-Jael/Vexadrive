using System;

namespace VexaDriveAPI.DTO.Notification
{
    public class NotificationDetailsDTO
    {
        public int NotificationId { get; set; }
        public int ServiceRequestId { get; set; }
        public string UserId { get; set; } = string.Empty;   // IdentityUser.Id (recipient)
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
