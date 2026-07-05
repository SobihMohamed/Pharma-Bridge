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
        [MinLength(3, ErrorMessage = "Full Name must be at least 3 characters")]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters")]
        public string FullName { get; set; }

        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Phone must be a starts wtih (010, 011, 012, 015) with 11 digits")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
        
        // Note: Email update should be handled in a separate Auth/Identity endpoint
    }
}
