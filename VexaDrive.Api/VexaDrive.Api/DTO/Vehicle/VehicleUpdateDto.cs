using System.ComponentModel.DataAnnotations;
 
namespace VexaDriveAPI.DTO.VehicleDTO
{
    public class VehicleUpdateDTO
    {
        [Required]
        public int VehicleId { get; set; }
 
        public string? Model { get; set; }
        public string? NumberPlate { get; set; }
        public string? Type { get; set; }
        public string? Color { get; set; }
 
        public int? OwnerId { get; set; }
    }
}