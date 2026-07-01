using System;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class PharmacyOwnerProfileDto
    {
        public int Id { get; set; }
        public string PharmacyName { get; set; } = null!;
        public string? Area { get; set; }
        public decimal AverageRating { get; set; }
        public int CompleteOrderCount { get; set; }
        public string Status { get; set; } = null!;
        public bool Is24Hours { get; set; }
        public string? OpenTime { get; set; }
        public string? CloseTime { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? TextAddress { get; set; }
        public string LicenseNumber { get; set; } = null!;
        public string? LicenseImageUrl { get; set; } 
        public string? RejectedReasons { get; set; }
        public string? ContactPhone { get; set; }
        public string OwnerName { get; set; } = null!;
        public string OwnerEmail { get; set; } = null!;
    }
}
