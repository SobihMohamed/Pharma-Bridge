using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PatientAddresses
{
    public class CreatePatientAddressDto
    {
        [Required]
        [MaxLength(250)]
        public string AddressLine { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }
        
        public decimal Latitude { get; set; }
        
        public decimal Longitude { get; set; }
        public bool IsDefault { get; set; }

        [Required]
        public string PatientProfileId { get; set; }
    }
}
