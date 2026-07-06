using PharmaBridge.Shared.DTOs.Admin.Dashboard;

namespace PharmaBridge.Abstraction.IServices.Admin
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetDashboardAsync();
    }
}