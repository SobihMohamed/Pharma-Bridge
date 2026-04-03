using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class AdminPharmacyDetailsDto : PharmacyDto
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        // public string? Address { get; set; } it already exist but in this get the all string
    
        public string LicenseNumber { get; set; }
        public string Status { get; set; }
        public string? RejectedReasons { get; set; }
        public string? ContactPhone { get; set; }
    
        // Flattening owner data
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
    }
}
