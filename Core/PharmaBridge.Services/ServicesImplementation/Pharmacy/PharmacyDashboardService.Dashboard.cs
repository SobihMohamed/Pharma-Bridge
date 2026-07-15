using Microsoft.AspNetCore.Http;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Services.Specifications.Dashboard;
using PharmaBridge.Services.Specifications.PharmaOwners;
using PharmaBridge.Shared.DTOs.Dashboard;
using System.Security.Claims;
using PharmacyEntity = PharmaBridge.Domain.Models.Pharma_Requests.Pharmacy;

namespace PharmaBridge.Services.ServicesImplementation.Pharmacy
{
    public partial class PharmacyDashboardService
    {
        public async Task<PharmacyDashboardDto> GetDashboardAsync(int pharmacyId)
        {
            await ValidatePharmacyOwnerForDashboardAsync(pharmacyId);

            var pharmacyRepo = _unitOfWork.GetRepository<PharmaBridge.Domain.Models.Pharma_Requests.Pharmacy, int>();
            var pharmacy = await pharmacyRepo.GetByIdAsync(pharmacyId);
            // ── Date ranges ───────────────────────────────────────────────
            var now = DateTime.UtcNow;
            var currentStart = now.StartOfWeek();
            var currentEnd = now;
            var prevStart = currentStart.AddDays(-7);
            var prevEnd = currentStart.AddTicks(-1);
            var last7Days = now.Date.AddDays(-6);

            var currentRating = pharmacy?.AverageRating ?? 0;

            var currentBids = await CountActiveBidsAsync(pharmacyId, currentStart, currentEnd);

            var prevBids = await CountActiveBidsAsync(pharmacyId, prevStart, prevEnd);

            var currentOrders = await GetCompletedOrdersAsync(pharmacyId, currentStart, currentEnd);

            var prevOrders = await GetCompletedOrdersAsync(pharmacyId, prevStart, prevEnd);

            var chartOrders = await GetChartOrdersAsync(pharmacyId, last7Days, currentEnd);

            var recentBids = await GetRecentAcceptedBidsAsync(pharmacyId);

            var recentOrders = await GetRecentDeliveredOrdersAsync(pharmacyId);

            

            // ── Calculations ───────────────────────────────────────────────
            var currentRevenue = currentOrders.Sum(o => o.Amount);
            var prevRevenue = prevOrders.Sum(o => o.Amount);
            var currentCompleted = currentOrders.Count;
            var prevCompleted = prevOrders.Count;
            var currentNewPatients = currentOrders.Select(o => o.PatientProfileId).Distinct().Count();
            var prevNewPatients = prevOrders.Select(o => o.PatientProfileId).Distinct().Count();

            return new PharmacyDashboardDto
            {
                ActiveBids = currentBids,
                Revenue = currentRevenue,
                NewPatients = currentNewPatients,
                CompletedOrders = currentCompleted,
                AverageRating = currentRating,
                ActiveBidsGrowth = CalculateGrowth(currentBids, prevBids),
                RevenueGrowth = CalculateGrowth(currentRevenue, prevRevenue),
                NewPatientsGrowth = CalculateGrowth(currentNewPatients, prevNewPatients),
                CompletedOrdersGrowth = CalculateGrowth(currentCompleted, prevCompleted),

                RevenueChart = BuildRevenueChart(chartOrders, last7Days, now.Date),
                RecentActivities = BuildRecentActivities(recentBids, recentOrders)
            };
        }

        // =====================================================================
        // PRIVATE — Data Fetchers
        // =====================================================================

        private async Task<int> CountActiveBidsAsync(int pharmacyId, DateTime from, DateTime to)
        {
            var spec = new ActiveBidsSpecification(pharmacyId, from, to);
            return await _unitOfWork.GetRepository<Bid, int>().GetCountAsync(spec);
        }

        private async Task<IReadOnlyList<Order>> GetCompletedOrdersAsync(
            int pharmacyId, DateTime from, DateTime to)
        {
            var spec = new CompletedOrdersSpecification(pharmacyId, from, to);
            return await _unitOfWork.GetRepository<Order, int>().GetAllWithSpecAsync(spec);
        }

        private async Task<IReadOnlyList<Order>> GetChartOrdersAsync(
            int pharmacyId, DateTime from, DateTime to)
        {
            var spec = new RevenueChartSpecification(pharmacyId, from, to);
            return await _unitOfWork.GetRepository<Order, int>().GetAllWithSpecAsync(spec);
        }

        private async Task<IReadOnlyList<Bid>> GetRecentAcceptedBidsAsync(int pharmacyId)
        {
            var spec = new RecentActivityBidsSpecification(pharmacyId);
            return await _unitOfWork.GetRepository<Bid, int>().GetAllWithSpecAsync(spec);
        }

        private async Task<IReadOnlyList<Order>> GetRecentDeliveredOrdersAsync(int pharmacyId)
        {
            var spec = new RecentActivityOrdersSpecification(pharmacyId);
            return await _unitOfWork.GetRepository<Order, int>().GetAllWithSpecAsync(spec);
        }

        // =====================================================================
        // PRIVATE — Calculations
        // =====================================================================

        private static decimal CalculateGrowth(decimal current, decimal previous)
        {
            if (previous == 0) return current > 0 ? 100m : 0m;
            return Math.Round((current - previous) / previous * 100, 1);
        }

        private static decimal CalculateGrowth(int current, int previous)
            => CalculateGrowth((decimal)current, (decimal)previous);

        private static List<RevenueChartItemDto> BuildRevenueChart(
            IReadOnlyList<Order> orders, DateTime from, DateTime to)
        {
            var grouped = orders
                .Where(o => o.DeliveredAt.HasValue)
                .GroupBy(o => o.DeliveredAt!.Value.Date)
                .ToDictionary(g => g.Key, g => g.Sum(o => o.Amount));

            var chart = new List<RevenueChartItemDto>();
            for (var day = from.Date; day <= to.Date; day = day.AddDays(1))
            {
                chart.Add(new RevenueChartItemDto
                {
                    Date = day.ToString("yyyy-MM-dd"),
                    Revenue = grouped.TryGetValue(day, out var rev) ? rev : 0m
                });
            }
            return chart;
        }

        private static List<RecentActivityDto> BuildRecentActivities(
            IReadOnlyList<Bid> recentBids,
            IReadOnlyList<Order> recentOrders)
        {
            var bidActivities = recentBids.Select(b => new RecentActivityDto
            {
                Title = "Bid Accepted",
                Description = $"Your bid for prescription #{b.PrescriptionRequestId} was accepted.",
                CreatedAt = b.RespondedAt ?? b.SubmittedAt,
                Type = "BidAccepted"
            });

            var orderActivities = recentOrders.Select(o => new RecentActivityDto
            {
                Title = "Order Completed",
                Description = $"Order #{o.Id} was successfully delivered.",
                CreatedAt = o.DeliveredAt ?? o.CreatedAt,
                Type = "OrderCompleted"
            });

            return bidActivities
                .Concat(orderActivities)
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToList();
        }

        // =====================================================================
        // PRIVATE — Auth
        // =====================================================================

        private async Task ValidatePharmacyOwnerForDashboardAsync(int pharmacyId)
        {
            var currentUserId = _httpContextAccessorField.HttpContext?
                .User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
                throw new UnAuthorizedCustomeException();

            var pharmacy = await _unitOfWork
                .GetRepository<PharmacyEntity, int>()
                .GetByIdAsync(pharmacyId);

            if (pharmacy is null || pharmacy.IsDeleted)
                throw new NotFoundCutomeException($"Pharmacy {pharmacyId} not found.");
         
            var ownerRepo = _unitOfWork.GetRepository<PharmaOwner, string>();
            var ownerSpec = new PharmaOwnerByAppUserIdSpecification(currentUserId);

            var currentOwner = await ownerRepo.GetByIdWithSpecAsync(ownerSpec);

            if (currentOwner == null)
                throw new UnAuthorizedCustomeException();

            if (pharmacy.PharmaOwnerId != currentOwner.Id)
                throw new UnAuthorizedCustomeException();


            if (pharmacy.Status == PharmaBridge.Shared.EnumHelper.PharmaEnums.PharmacyStatus.Pending)
                throw new BadRequestCustomeException("Your pharmacy account is not approved yet.");

            if (pharmacy.Status == PharmaBridge.Shared.EnumHelper.PharmaEnums.PharmacyStatus.Blocked)
                throw new BadRequestCustomeException("Your pharmacy account is blocked.");
        }
    }



    // ── Date helper ───────────────────────────────────────────────────────────
    internal static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }
    }
}