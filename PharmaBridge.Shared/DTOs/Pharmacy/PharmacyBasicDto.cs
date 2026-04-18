using System;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class PharmacyBasicDto
    {
        public string PharmacyName { get; set; } = null!;
        public string? Area { get; set; } 
        public decimal AverageRating { get; set; }
        public bool IsOpen { get; set; }
    }
}
