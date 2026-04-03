using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaPerformSnapshot
{
    public class PharmaPerformSnapshotDto
    {
       public int Id { get; set; }
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
        public string PeriodType { get; set; } 

        // Flattening:
        public string PharmacyName { get; set; }
        public int PharmacyId { get; set; }

        public int TotalBids { get; set; }
        public int WonOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }

        // KPI
        public decimal CompletionRate { get; set; }

        public decimal TotalRevenue { get; set; }
        
        // owner and admin only
        public decimal TotalPlatformFee { get; set; }

        public DateTime ComputedAt { get; set; }
    }
}
