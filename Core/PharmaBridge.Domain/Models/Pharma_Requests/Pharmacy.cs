using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.Pharma_Requests
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

        // 12 - Bid (Many) To (1) Pharmacy (Create Bid)
        public virtual ICollection<Bid> Bids { get; set; } = new HashSet<Bid>();

        // 16 - Pharmacy (1) To (Many) Order (Get)
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
