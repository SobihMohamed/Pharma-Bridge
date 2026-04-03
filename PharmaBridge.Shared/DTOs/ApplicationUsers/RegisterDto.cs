using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.ApplicationUsers
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]{2,}@[a-zA-Z0-9.-]{2,}\.[a-zA-Z]{3,}$", ErrorMessage = "Invalid email format. Please use a valid domain (e.g., user@example.com)")]
        public string Email { get; set; } = null!;
        
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = null!;
        
        [Required, Phone]
        public string PhoneNumber { get; set; } = null!;

        The Missing Link: Which type of user is registering?
        [Required(ErrorMessage = "Please specify the account type (Patient or Pharmacy Owner).")]
        [Range(2, 3, ErrorMessage = "Invalid Role. Registration is only allowed for Patients and Pharmacy Owners.")]
        public UserRole Role { get; set; }
    }
}
