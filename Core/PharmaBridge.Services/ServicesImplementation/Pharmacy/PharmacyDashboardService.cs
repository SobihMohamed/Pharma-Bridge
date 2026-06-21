using PharmaBridge.Abstraction.IServices.CurrentUser;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Domain.Contracts.SpecificReposPattern;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Services.Specifications.Pharmacy;
using PharmaBridge.Shared.DTOs.PharmaPerformSnapshot;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Threading.Tasks;

namespace PharmaBridge.Services.ServicesImplementation.Pharmacy
{
    public class PharmacyDashboardService : IPharmacyDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly ICurrentUserService _currentUser;

        public PharmacyDashboardService(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _currentUser = currentUser;
        }

        public async Task<PharmaPerformSnapshotDto> GetMyPerformanceSnapshotAsync(int pharmacyId)
        {
            if (!_currentUser.IsAdmin)
            {
                if (_currentUser.IsPharmacyOwner)
                {
                    var ownerPharmacyId = await _currentUser.GetPharmacyIdAsync();
                    if (pharmacyId != ownerPharmacyId)
                        throw new UnAuthorizedCustomeException();
                }
                else
                {
                    throw new UnAuthorizedCustomeException();
                }
            }

            // 1. Fetch Pharmacy for details like Name and AverageRating
            var pharmacyRepo = _unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>();
            var pharmacy = await pharmacyRepo.GetByIdAsync(pharmacyId)
                ?? throw new NotFoundCutomeException($"Pharmacy with Id '{pharmacyId}' was not found.");

            // 2. Count today's completed and pending orders using GenericRepo specification
            var completedOrdersCount = await _orderRepository.GetCountAsync(new PharmacyOrdersByStatusSpec(pharmacyId, OrderStatus.Completed));
            var pendingOrdersCount = await _orderRepository.GetCountAsync(new PharmacyOrdersByStatusSpec(pharmacyId, OrderStatus.Pending));
            var cancelledOrdersCount = await _orderRepository.GetCountAsync(new PharmacyOrdersByStatusSpec(pharmacyId, OrderStatus.Cancelled));
            
            // Note: TotalBids and WonOrders aren't requested directly for today's summary but can be filled with 0s for now, or computed if needed.
            // The user requested: Today's Orders Count (Status: Completed/Pending). Total Revenue. Average Rating.
            
            // 3. Calculate DB-side Total Revenue using the specific repo
            var totalRevenue = await _orderRepository.GetTotalRevenueAsync(pharmacyId);

            // 4. Map to DTO
            return new PharmaPerformSnapshotDto
            {
                Id = 0, // This is just a snapshot, might not be persisted in this method yet.
                PharmacyId = pharmacy.Id,
                PharmacyName = pharmacy.PharmacyName,
                PeriodStart = DateOnly.FromDateTime(DateTime.UtcNow),
                PeriodEnd = DateOnly.FromDateTime(DateTime.UtcNow),
                PeriodType = "Daily",
                CompletedOrders = completedOrdersCount,
                CancelledOrders = cancelledOrdersCount,
                PendingOrders = pendingOrdersCount,
                TotalBids = 0, 
                WonOrders = completedOrdersCount, 
                CompletionRate = (completedOrdersCount + cancelledOrdersCount) > 0 
                    ? Math.Round((decimal)completedOrdersCount / (completedOrdersCount + cancelledOrdersCount) * 100, 2) 
                    : 0m,
                TotalRevenue = totalRevenue,
                AverageRating = pharmacy.AverageRating,
                ComputedAt = DateTime.UtcNow,
            };
        }
    }
}
