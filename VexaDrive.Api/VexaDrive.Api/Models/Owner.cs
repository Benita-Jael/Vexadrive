using System.ComponentModel.DataAnnotations;
 
namespace VexaDriveAPI.Models
{
    public class Owner
    {
        [Key]
        public int OwnerId { get; set; }
 
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
 
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
 
        [Required]
        [Phone]
        [StringLength(20)]
        public string ContactNumber { get; set; } = string.Empty;
 
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
 
        // Navigation property
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}