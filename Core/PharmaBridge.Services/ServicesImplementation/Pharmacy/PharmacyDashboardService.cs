using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Domain.Contracts.SpecificReposPattern;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Services.Specifications.Dashboard;
using PharmaBridge.Shared.DTOs.PharmaPerformSnapshot;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Threading.Tasks;

namespace PharmaBridge.Services.ServicesImplementation.Pharmacy
{
    public partial class PharmacyDashboardService : IPharmacyDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;

        public PharmacyDashboardService(IUnitOfWork unitOfWork, IOrderRepository orderRepository)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
        }

        public async Task<PharmaPerformSnapshotDto> GetMyPerformanceSnapshotAsync(int pharmacyId, string userId, string role)
        {
            var pharmacy = await GetValidPharmacyAsync(pharmacyId, userId, role);

            var orderRepo = _unitOfWork.GetRepository<Order, int>();

            // Count today's completed orders
            var completedSpec = new TodayOrdersCountByStatusSpec(pharmacyId, OrderStatus.Completed);
            var completedCount = await orderRepo.GetCountAsync(completedSpec);

            // Count today's pending orders
            var pendingSpec = new TodayOrdersCountByStatusSpec(pharmacyId, OrderStatus.Pending);
            var pendingCount = await orderRepo.GetCountAsync(pendingSpec);

            // Total revenue (server-side SUM)
            var totalRevenue = await _orderRepository.GetTotalRevenueAsync(pharmacyId);

            return new PharmaPerformSnapshotDto
            {
                PharmacyName = pharmacy.PharmacyName,
                PharmacyId = pharmacyId,
                CompletedOrders = completedCount,
                PendingOrders = pendingCount,
                TotalRevenue = totalRevenue,
                AverageRating = pharmacy.AverageRating,
                ComputedAt = DateTime.UtcNow
            };
        }
    }
}
