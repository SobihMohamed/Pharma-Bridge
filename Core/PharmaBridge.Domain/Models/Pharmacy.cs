using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Domain.Models
{
    public class Pharmacy : BaseEntity<int>
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

        // 8- pharmacies (Many) To (1) Pharma_Owner (owned)
        public string PharmaOwnerId { get; set; }
        public  User.PharmaOwner PharmaOwner { get; set; }

        // 6- PharmacyRating (Many) To (1) pharmacies (Has)
        public  ICollection<PharmacyRating> Ratings { get; set; } = new HashSet<PharmacyRating>();

        // 7- Pharma_Perform_Snapshot (Many) To (1) pharmacies (Has)
        public  ICollection<PharmaPerformSnapshot> Snapshots { get; set; } = new HashSet<PharmaPerformSnapshot>(); 

    }
}
