using System.ComponentModel.DataAnnotations;

namespace VexaDriveAPI.DTO.VehicleDTO
{
    public class VehicleCreateDTO
    {
        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        public string NumberPlate { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty; // Car, Bike

        [Required]
        public string Color { get; set; } = string.Empty;

        // Link to IdentityUser (Customer)
        public string? CustomerUserId { get; set; }
    }
}
