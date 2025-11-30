using System.ComponentModel.DataAnnotations;

namespace VexaDriveAPI.DTO.AuthDTO
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Password & Confirm Password is not matching")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        // Contact number collected during registration (required per new requirements)
        [Required]
        public string ContactNumber { get; set; } = string.Empty;
    }
}
