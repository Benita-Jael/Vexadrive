using System.ComponentModel.DataAnnotations;
 
namespace VexaDriveAPI.DTO.VehicleDTO
{
    public class VehicleCreateDTO
    {
        [Required]
        public string Model { get; set; } = string.Empty;
 
        public string? NumberPlate { get; set; }
 
        [Required]
        public string Type { get; set; } = string.Empty; // Car, Bike
 
        [Required]
        public string Color { get; set; } = string.Empty;
 
        [Required]
        public int OwnerId { get; set; }  
    }
}