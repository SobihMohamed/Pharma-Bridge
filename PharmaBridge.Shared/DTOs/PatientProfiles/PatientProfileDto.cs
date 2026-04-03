using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PatientProfiles
{
    public class PatientProfileDto
    {
       public string Id { get; set; } // PatientProfile ID
        
        // Flattening from IdentityUser
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
