using System.ComponentModel.DataAnnotations;
 
namespace VexaDriveAPI.DTO.OwnerDTO
{
    public class OwnerCreateDTO
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
 
        public string? LastName { get; set; }
 
        [Phone]
        [Required]
        public string ContactNumber { get; set; } = string.Empty;
 
        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}