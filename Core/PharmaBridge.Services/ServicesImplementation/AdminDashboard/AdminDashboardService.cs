using PharmaBridge.Abstraction.IServices.Admin;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Services.Specifications.AdminDashboard;
using PharmaBridge.Shared.DTOs.Admin.Dashboard;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;

namespace PharmaBridge.Services.ServicesImplementation.Admin
{
    public class AdminDashboardService(IUnitOfWork unitOfWork) : IAdminDashboardService
    {
        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var stats = await BuildStatsAsync();
            var modules = BuildModules();
            var activity = await BuildRecentActivityAsync();

            return new AdminDashboardDto
            {
                Statistics = stats,
                Modules = modules,
                RecentActivity = activity
            };
        }


        private async Task<AdminStatsDto> BuildStatsAsync()
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var thisMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastMonthStart = thisMonthStart.AddMonths(-1);
            var expiresWithin = now.AddHours(2);

            var patientRepo = unitOfWork.GetRepository<PatientProfile, string>();
            var bidRepo = unitOfWork.GetRepository<Bid, int>();
            var requestRepo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();
            var orderRepo = unitOfWork.GetRepository<Order, int>();

            var (
                totalPatients,
                thisMonthPatients,
                lastMonthPatients,
                activeBids,
                expiringBids,
                todayOrders,
                completedOrders,
                nonCancelledOrders
            ) = (
                await patientRepo.GetCountAsync(new AdminTotalPatientsCountSpec()),
                await patientRepo.GetCountAsync(new AdminPatientsByPeriodCountSpec(thisMonthStart, now)),
                await patientRepo.GetCountAsync(new AdminPatientsByPeriodCountSpec(lastMonthStart, thisMonthStart)),
                await bidRepo.GetCountAsync(new AdminActiveBidsCountSpec()),
                await requestRepo.GetCountAsync(new AdminExpiringRequestsWithBidsCountSpec(expiresWithin)),
                await orderRepo.GetCountAsync(new AdminTodayOrdersCountSpec(todayStart)),
                await orderRepo.GetCountAsync(new AdminCompletedOrdersCountSpec()),
                await orderRepo.GetCountAsync(new AdminNonCancelledOrdersCountSpec())
            );

            return new AdminStatsDto
            {
                TotalPatients = totalPatients,
                PatientGrowthPercent = CalculateGrowthPercent(thisMonthPatients, lastMonthPatients),
                ActiveBidsCount = activeBids,
                ExpiringBidsCount = expiringBids,
                TodayOrdersCount = todayOrders,
                FulfillmentRatePercent = CalculateFulfillmentRate(completedOrders, nonCancelledOrders)
            };
        }

        private static IReadOnlyList<AdminModuleDto> BuildModules() =>
        [
            new()
            {
                Name            = "Patients",
                Description     = "Manage patient accounts, profiles, and activity.",
                ImageId         = "module_patients",
                PrimaryButton   = new() { Label = "Open Module", Action = "admin/patients" },
                SecondaryButton = new() { Label = "Add Patient",  Action = "admin/patients/create" }
            },
            new()
            {
                Name            = "Pharmacies",
                Description     = "Approve, block, and monitor pharmacy registrations.",
                ImageId         = "module_pharmacies",
                PrimaryButton   = new() { Label = "Open Module",       Action = "admin/pharmacies" },
                SecondaryButton = new() { Label = "Pending Approvals", Action = "admin/pharmacies?status=Pending" }
            },
            new()
            {
                Name            = "Orders",
                Description     = "Monitor all platform orders and their statuses.",
                ImageId         = "module_orders",
                PrimaryButton   = new() { Label = "Open Module",    Action = "admin/orders" },
                SecondaryButton = new() { Label = "Today's Orders", Action = "admin/orders?filter=today" }
            },
            new()
            {
                Name            = "Prescription Requests",
                Description     = "View all patient prescription requests across the platform.",
                ImageId         = "module_prescriptions",
                PrimaryButton   = new() { Label = "Open Module",     Action = "admin/prescription-requests" },
                SecondaryButton = new() { Label = "Active Requests", Action = "admin/prescription-requests?status=HasBids" }
            },
            new()
            {
                Name            = "Bids",
                Description     = "Monitor bids submitted by pharmacies for prescription requests.",
                ImageId         = "module_bids",
                PrimaryButton   = new() { Label = "Open Module", Action = "admin/bids" },
                SecondaryButton = new() { Label = "Active Bids", Action = "admin/bids?status=Pending" }
            }
        ];

        private async Task<IReadOnlyList<AdminActivityDto>> BuildRecentActivityAsync()
        {
            var orderRepo = unitOfWork.GetRepository<Order, int>();
            var bidRepo = unitOfWork.GetRepository<Bid, int>();
            var requestRepo = unitOfWork.GetRepository<PrescriptionRequestEntity, int>();

            var (orders, bids, requests) = (
                await orderRepo.GetAllWithSpecAsync(new AdminRecentOrdersSpec(take: 4)),
                await bidRepo.GetAllWithSpecAsync(new AdminRecentBidsSpec(take: 4)),
                await requestRepo.GetAllWithSpecAsync(new AdminRecentRequestsSpec(take: 4))
            );

            var activities = new List<AdminActivityDto>();

            activities.AddRange(orders.Select(MapOrderToActivity));
            activities.AddRange(bids.Select(MapBidToActivity));
            activities.AddRange(requests.Select(MapRequestToActivity));

            return activities
                .OrderByDescending(a => a.ActivityAt)
                .Take(10)
                .ToList()
                .AsReadOnly();
        }


        private static AdminActivityDto MapOrderToActivity(Order order) => new()
        {
            ActivityAt = order.CreatedAt,
            Category = "Order",
            Description = $"Order #{order.Id} — {order.OrderStatus}",
            Status = order.OrderStatus switch
            {
                OrderStatus.Completed => "Completed",
                OrderStatus.Cancelled => "Cancelled",
                OrderStatus.InTransit => "In Transit",
                _ => "Pending Review"
            },
            PerformedBy = order.PatientProfile?.ApplicationUser?.FullName ?? "Patient"
        };

        private static AdminActivityDto MapBidToActivity(Bid bid) => new()
        {
            ActivityAt = bid.SubmittedAt,
            Category = "Bid",
            Description = $"Bid #{bid.Id} submitted for Request #{bid.PrescriptionRequestId}",
            Status = bid.Status switch
            {
                BidStatus.Accepted => "Completed",
                BidStatus.Rejected => "Cancelled",
                _ => "Pending Review"
            },
            PerformedBy = bid.Pharmacy?.PharmacyName ?? "Pharmacy"
        };

        private static AdminActivityDto MapRequestToActivity(PrescriptionRequestEntity request) => new()
        {
            ActivityAt = request.CreatedAt,
            Category = "Prescription Request",
            Description = $"Request #{request.Id} — {request.MedicineName ?? "Image Prescription"}",
            Status = request.Status switch
            {
                PrescriptionStatus.Closed => "Completed",
                PrescriptionStatus.Cancelled => "Cancelled",
                _ => "Pending Review"
            },
            PerformedBy = "Patient"
        };


        private static decimal CalculateGrowthPercent(int current, int previous)
        {
            if (previous == 0)
                return current > 0 ? 100m : 0m;

            return Math.Round((decimal)(current - previous) / previous * 100, 1);
        }

        private static decimal CalculateFulfillmentRate(int completed, int nonCancelled)
        {
            if (nonCancelled == 0) return 0m;
            return Math.Round((decimal)completed / nonCancelled * 100, 1);
        }
    }
}