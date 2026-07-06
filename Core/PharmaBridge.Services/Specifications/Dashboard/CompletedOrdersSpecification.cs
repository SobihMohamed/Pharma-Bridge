using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;

namespace PharmaBridge.Services.Specifications.Dashboard
{
    public class CompletedOrdersSpecification : BaseSpecifications<Order, int>
    {
        public CompletedOrdersSpecification(int pharmacyId, DateTime from, DateTime to)
            : base(o => o.PharmacyId == pharmacyId
                     && o.OrderStatus == OrderStatus.Completed
                     && o.DeliveredAt >= from
                     && o.DeliveredAt <= to)
        {
        }
    }
}
