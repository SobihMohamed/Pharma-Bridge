using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PatientAddresses
{
    public class CreatePatientAddressDto
    {
        public string AddressLine { get; set; }
        public string City { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
