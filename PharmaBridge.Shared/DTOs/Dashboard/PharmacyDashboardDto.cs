namespace PharmaBridge.Shared.DTOs.Dashboard
{
    public class PharmacyDashboardDto
    {
        // ── Core Metrics ──────────────────────────────────────────────────
        public int ActiveBids { get; set; }
        public decimal Revenue { get; set; }
        public int NewPatients { get; set; }
        public int CompletedOrders { get; set; }

        public decimal AverageRating { get; set; }
        // ── Growth % (current week vs previous week) ──────────────────────
        public decimal ActiveBidsGrowth { get; set; }
        public decimal RevenueGrowth { get; set; }
        public decimal NewPatientsGrowth { get; set; }
        public decimal CompletedOrdersGrowth { get; set; }

        // ── Charts & Activity ─────────────────────────────────────────────
        public List<RevenueChartItemDto> RevenueChart { get; set; } = new();
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
    }
}