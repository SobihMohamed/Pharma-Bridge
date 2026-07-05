namespace PharmaBridge.Shared.DTOs.Admin.Dashboard
{
    public class AdminStatsDto
    {
        public int TotalPatients { get; set; }
        public decimal PatientGrowthPercent { get; set; }

        public int ActiveBidsCount { get; set; }
        public int ExpiringBidsCount { get; set; }

        public int TodayOrdersCount { get; set; }
        public decimal FulfillmentRatePercent { get; set; }
    }
}