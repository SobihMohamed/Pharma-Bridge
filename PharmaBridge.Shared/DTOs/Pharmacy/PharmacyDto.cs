using System;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class PharmacyDto
    {
        public string PharmacyName { get; set; } = null!;
        public string? Area { get; set; } 
        public string? TextAddress { get; set; }
        public decimal AverageRating { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsOpen { get; set; }
    }
}
