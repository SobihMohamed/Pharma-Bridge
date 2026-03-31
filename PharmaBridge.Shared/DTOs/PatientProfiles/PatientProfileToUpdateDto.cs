using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PatientProfiles
{
    public class PatientProfileToUpdateDto
    {
        [Required]
        public string Id { get; set; } 

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
