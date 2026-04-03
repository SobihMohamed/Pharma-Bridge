using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PatientAddresses
{
    public class CreatePatientAddressDto
    {
        [Required(ErrorMessage = "Address line is required")]
        [MaxLength(250)]
        public string AddressLine { get; set; }

        [Required(ErrorMessage = "City is required")]
        [MaxLength(100)]
        public string City { get; set; }

        // Adding Geographic Boundaries to prevent Map Errors
        [Required]
        [Range(-90.0, 90.0, ErrorMessage = "Latitude must be between -90 and 90 degrees.")]
        public decimal Latitude { get; set; }

        [Required]
        [Range(-180.0, 180.0, ErrorMessage = "Longitude must be between -180 and 180 degrees.")]
        public decimal Longitude { get; set; }

        public bool IsDefault { get; set; }

        // REMOVED: PatientProfileId 
        // Reason: Security. The Backend MUST extract this from the authenticated user's JWT Token.
    }
}
