using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.Pharma_Requests
{
    public class PharmaPerformSnapshot : BaseEntity<int>
    {
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
        public SnapshotPeriodType PeriodType { get; set; }
        public int TotalBids { get; set; } = 0;
        public int WonOrders { get; set; } = 0;
        public int CompletedOrders { get; set; } = 0;
        public int CancelledOrders { get; set; } = 0;
        public decimal CompletionRate { get; set; } = 0.00m;
        public decimal TotalRevenue { get; set; } = 0.00m;
        public decimal TotalPlatformFee { get; set; } = 0.00m;
        public DateTime ComputedAt { get; set; } = DateTime.UtcNow;

        public int PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
    }
}
