namespace PharmaBridge.Shared.DTOs.Admin.Dashboard
{
    public class AdminDashboardDto
    {
        public AdminStatsDto Statistics { get; set; } = new();
        public IReadOnlyList<AdminModuleDto> Modules { get; set; } = [];
        public IReadOnlyList<AdminActivityDto> RecentActivity { get; set; } = [];
    }
}