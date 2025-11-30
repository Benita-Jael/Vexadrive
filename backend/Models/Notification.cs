using System;
using System.ComponentModel.DataAnnotations;

namespace VexaDriveAPI.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public int ServiceRequestId { get; set; }
        public ServiceRequest? ServiceRequest { get; set; }

        [Required]
        public required string UserId { get; set; } // IdentityUser.Id (recipient)

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Message { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
