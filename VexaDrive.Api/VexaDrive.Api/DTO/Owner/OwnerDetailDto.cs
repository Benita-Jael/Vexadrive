namespace VexaDriveAPI.DTO.OwnerDTO
{
    public class OwnerDetailsDTO
    {
        public int OwnerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}