using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;

namespace PharmaBridge.Services.Specifications.Dashboard
{
    public class TodayOrdersCountByStatusSpec : BaseSpecifications<Order, int>
    {
        public TodayOrdersCountByStatusSpec(int pharmacyId, OrderStatus status)
            : base(o => o.PharmacyId == pharmacyId && 
                        o.OrderStatus == status &&
                        o.CreatedAt.Date == DateTime.UtcNow.Date)
        {
        }
    }
}
