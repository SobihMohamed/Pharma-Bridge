using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Domain.Models
{
    public class Pharmacy : BaseEntity<string>
    {
        public string PharmacyName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string? LicenseImageUrl { get; set; }
        public PharmacyStatus Status { get; set; } = PharmacyStatus.Pending;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public bool Is24Hours { get; set; } = false;
        public string? TextAddress { get; set; }
        public string? ContactPhone { get; set; }
        public decimal AverageRating { get; set; } = 0.00m;
        public int CompleteOrderCount { get; set; } = 0;
        public string? RejectedReasons { get; set; } 

    }
}
