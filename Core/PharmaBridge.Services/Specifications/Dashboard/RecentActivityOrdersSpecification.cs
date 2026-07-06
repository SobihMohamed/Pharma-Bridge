using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;

namespace PharmaBridge.Services.Specifications.Dashboard
{
    public class RecentActivityOrdersSpecification : BaseSpecifications<Order, int>
    {
        public RecentActivityOrdersSpecification(int pharmacyId)
            : base(o => o.PharmacyId == pharmacyId
                     && o.OrderStatus == OrderStatus.Completed)
        {
            AddInclude(o => o.PatientProfile);
            AddOrderBy(o => o.DeliveredAt!, isDescending: true);
            ApplyPaging(PageSize: 10, PageIndex: 1);
        }
    }
}