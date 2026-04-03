using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PatientProfiles
{
    public class PatientProfileToUpdateDto
    {
        // REMOVED: public string Id { get; set; }
        // Security: The backend must use the logged-in user's Token to find their profile.

        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
        
        // Note: Email update should be handled in a separate Auth/Identity endpoint
    }
}
