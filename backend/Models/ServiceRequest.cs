using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VexaDriveAPI.Models
{
    public class ServiceRequest
    {
        [Key]
        public int ServiceRequestId { get; set; }

        [Required]
        public required string CustomerUserId { get; set; }  // IdentityUser.Id

        [Required]
        public int VehicleId { get; set; }
        public required Vehicle? Vehicle { get; set; }

        [Required]
        public required string ProblemDescription { get; set; }

        [Required]
        public required string ServiceAddress { get; set; }

        [Required]
        public DateTime ServiceDate { get; set; }

        public ServiceStatus Status { get; set; } = ServiceStatus.RequestCreated;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EstimatedDeliveryDate { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public Bill? Bill { get; set; }
    }
}
