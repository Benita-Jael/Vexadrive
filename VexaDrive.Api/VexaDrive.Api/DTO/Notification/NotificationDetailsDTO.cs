using System;

namespace VexaDriveAPI.DTO.Notification
{
    public class NotificationDetailsDTO
    {
        public int NotificationId { get; set; }
        public int ServiceRequestId { get; set; }
        public string UserId { get; set; }   // IdentityUser.Id (recipient)
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
