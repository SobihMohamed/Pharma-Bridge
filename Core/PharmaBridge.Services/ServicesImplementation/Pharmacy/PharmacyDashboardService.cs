using Microsoft.AspNetCore.Http;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Domain.Contracts.SpecificReposPattern;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Services.Specifications.Dashboard;
using PharmaBridge.Services.Specifications.PharmaOwners;
using PharmaBridge.Shared.DTOs.PharmaPerformSnapshot;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PharmaBridge.Services.ServicesImplementation.Pharmacy
{
    public partial class PharmacyDashboardService : IPharmacyDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessorField;

        public PharmacyDashboardService(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _httpContextAccessorField = httpContextAccessor;
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
        private async Task EnsurePharmacyOwnershipAsync(int pharmacyId)
        {
            var currentUserId = _httpContextAccessorField.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
                throw new UnAuthorizedCustomeException();

            var ownerRepo = _unitOfWork.GetRepository<PharmaOwner, string>();
            var ownerSpec = new PharmaOwnerByAppUserIdSpecification(currentUserId);

            var currentOwner = await ownerRepo.GetByIdWithSpecAsync(ownerSpec);

            if (currentOwner == null)
                throw new UnAuthorizedCustomeException();

            var pharmacyRepo = _unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>();
            var pharmacy = await pharmacyRepo.GetByIdAsync(pharmacyId);

            if (pharmacy is null || pharmacy.IsDeleted)
                throw new NotFoundCutomeException($"Pharmacy with ID {pharmacyId} was not found.");

            if (pharmacy.PharmaOwnerId != currentOwner.Id)
                throw new UnAuthorizedCustomeException();

            if (pharmacy.Status == PharmacyStatus.Pending)
                throw new BadRequestCustomeException("Your pharmacy account is not approved yet. You cannot submit bids.");

            if (pharmacy.Status == PharmacyStatus.Blocked)
                throw new BadRequestCustomeException("Your pharmacy account is Blocked. You cannot submit bids.");
        }
    }
}
