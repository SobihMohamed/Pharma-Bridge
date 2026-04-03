using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PatientAddresses
{
    public class UpdatePatientAddressDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string AddressLine { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        // Adding Geographic Boundaries (Consistency with CreateDto)
        [Required]
        [Range(-90.0, 90.0, ErrorMessage = "Latitude must be between -90 and 90 degrees.")]
        public decimal Latitude { get; set; }

        [Required]
        [Range(-180.0, 180.0, ErrorMessage = "Longitude must be between -180 and 180 degrees.")]
        public decimal Longitude { get; set; }

        public bool IsDefault { get; set; }
    }
}
