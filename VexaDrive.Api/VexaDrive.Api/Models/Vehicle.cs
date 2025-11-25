using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace VexaDriveAPI.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }
 
        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;
 
        [Required]
        [MaxLength(50)]
        public string NumberPlate { get; set; } = string.Empty;
 
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // Car, Bike
 
        [Required]
        [MaxLength(255)]
        public string Color { get; set; } = string.Empty;
 
        // Foreign key
        [ForeignKey("OwnerId")]
        public int? OwnerId { get; set; }
 
        public Owner? Owner { get; set; }
    }
}