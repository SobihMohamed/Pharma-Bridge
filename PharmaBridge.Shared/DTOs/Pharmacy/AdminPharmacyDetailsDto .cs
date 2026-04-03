using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class AdminPharmacyDetailsDto : PharmacyDto
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? TextAddress { get; set; }
    
        public string LicenseNumber { get; set; }
        public string? LicenseImageUrl { get; set; }
        public string Status { get; set; }
        public string? RejectedReasons { get; set; }
        public string? ContactPhone { get; set; }
    
        // Flattening owner data
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
    }
}
