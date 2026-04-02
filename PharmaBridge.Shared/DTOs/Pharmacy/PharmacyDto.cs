using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class PharmacyDto
    {
        public int Id { get; set; }
        public string PharmacyName { get; set; }
        public string? TextAddress { get; set; }
        public decimal AverageRating { get; set; }
        public bool Is24Hours { get; set; }
        public string? LicenseImageUrl { get; set; }
    }
}
