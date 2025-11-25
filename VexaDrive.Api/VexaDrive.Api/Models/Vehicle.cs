using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VexaDriveAPI.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }

        [Required, MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string NumberPlate { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Type { get; set; } = string.Empty; // Car, Bike

        [Required, MaxLength(255)]
        public string Color { get; set; } = string.Empty;

        // Link to IdentityUser (Customer)
        [Required]
        public string CustomerUserId { get; set; }

        // Navigation property
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
