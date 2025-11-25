using System.ComponentModel.DataAnnotations;
 
namespace VexaDriveAPI.DTO.OwnerDTO
{
    public class OwnerUpdateDTO
    {
        [Required]
        public int OwnerId { get; set; }  // required for matching
 
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
 
        [Phone]
        public string? ContactNumber { get; set; }
 
        [EmailAddress]
        public string? Email { get; set; }
    }
}