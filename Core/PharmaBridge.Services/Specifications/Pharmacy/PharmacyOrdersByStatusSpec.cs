using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;

namespace PharmaBridge.Services.Specifications.Pharmacy
{
    public class PharmacyOrdersByStatusSpec : BaseSpecifications<Order, int>
    {
        public PharmacyOrdersByStatusSpec(int pharmacyId, OrderStatus status)
            : base(o => o.PharmacyId == pharmacyId && 
                        o.OrderStatus == status &&
                        o.CreatedAt.Date == DateTime.UtcNow.Date)
        {
        }
    }
}
