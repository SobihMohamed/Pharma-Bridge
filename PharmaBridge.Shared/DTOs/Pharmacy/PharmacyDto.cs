using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class PharmacyDto
    {
        public string PharmacyName { get; set; }
        public string? Address { get; set; } // show only first 10 char
        public decimal AverageRating { get; set; }
        
        // 1. Social Proof: To build trust with patients
        public int CompleteOrderCount { get; set; }

        public bool Is24Hours { get; set; }

        // 2. Added Operating Hours for non-24h pharmacies
        public string? OpenTime { get; set; } 
        public string? CloseTime { get; set; }

        // REMOVED: LicenseImageUrl
        // This is sensitive legal data and should only be visible to Admins.
    }
}
