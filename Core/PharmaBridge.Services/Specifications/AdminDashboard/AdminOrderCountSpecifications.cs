using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;

namespace PharmaBridge.Services.Specifications.AdminDashboard
{
    public sealed class AdminTodayOrdersCountSpec : BaseSpecifications<Order, int>
    {
        public AdminTodayOrdersCountSpec(DateTime todayUtcStart)
            : base(o => !o.IsDeleted && o.CreatedAt >= todayUtcStart) { }
    }

    public sealed class AdminCompletedOrdersCountSpec : BaseSpecifications<Order, int>
    {
        public AdminCompletedOrdersCountSpec()
            : base(o => !o.IsDeleted && o.OrderStatus == OrderStatus.Completed) { }
    }

    public sealed class AdminNonCancelledOrdersCountSpec : BaseSpecifications<Order, int>
    {
        public AdminNonCancelledOrdersCountSpec()
            : base(o => !o.IsDeleted && o.OrderStatus != OrderStatus.Cancelled) { }
    }
}