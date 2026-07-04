using System;
using PharmaBridge.Shared.DTOs.PharmaOwners;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class AdminPharmacyDetailsDto
    {
        public int Id { get; set; }
        public string PharmacyName { get; set; } = null!;
        public string LicenseNumber { get; set; } = null!;
        public string? LicenseImageUrl { get; set; }
        public string Status { get; set; } = null!;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool Is24Hours { get; set; }
        public string? OpenTime { get; set; }
        public string? CloseTime { get; set; }
        public string? TextAddress { get; set; }
        public string Area { get; set; } = null!;
        public string? ContactPhone { get; set; }
        public decimal AverageRating { get; set; }
        public int CompleteOrderCount { get; set; }
        public string? RejectedReasons { get; set; }
        public DateTime RegistrationDate { get; set; }

        public PharmaOwnerDetailsDto Owner { get; set; } = null!;
    }
}
