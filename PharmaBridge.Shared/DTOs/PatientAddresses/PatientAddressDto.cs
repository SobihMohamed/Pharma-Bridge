using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PatientAddresses
{
    public class PatientAddressDto
    {
        public int Id { get; set; } // CRITICAL for selection/update/delete
        public string AddressLine { get; set; }
        public string City { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsDefault { get; set; } // CRITICAL for UI UX
    }
}
