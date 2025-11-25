using System;

namespace VexaDriveAPI.DTO.Notification
{
    public class NotificationListDTO
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
