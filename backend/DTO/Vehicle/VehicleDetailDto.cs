namespace VexaDriveAPI.DTO.VehicleDTO
{
    public class VehicleDetailsDTO
    {
        public int VehicleId { get; set; }
        public string? Model { get; set; }
        public string? NumberPlate { get; set; }
        public string? Type { get; set; }
        public string? Color { get; set; }

        // IdentityUser link
        public string? CustomerUserId { get; set; }
        public string? CustomerEmail { get; set; }
    }
}
