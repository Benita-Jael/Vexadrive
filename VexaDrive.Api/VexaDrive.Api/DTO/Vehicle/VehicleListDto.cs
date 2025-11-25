namespace VexaDriveAPI.DTO.VehicleDTO
{
    public class VehicleListDTO
    {
        public int VehicleId { get; set; }
        public string? Model { get; set; }
        public string? NumberPlate { get; set; }
        public string? Type { get; set; }
        public string? Color { get; set; }

        public string? CustomerUserId { get; set; }
    }
}
