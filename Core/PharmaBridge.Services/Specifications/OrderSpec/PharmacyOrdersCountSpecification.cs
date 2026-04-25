using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.Common.Params.Order;

namespace PharmaBridge.Services.Specifications.OrderSpec
{
    public class PharmacyOrdersCountSpecification : BaseSpecifications<Order, int>
    {
        public PharmacyOrdersCountSpecification(int pharmacyId, OrderQueryParams queryParams)
            : base(order => order.PharmacyId == pharmacyId
                && (!queryParams.Status.HasValue || order.OrderStatus == queryParams.Status.Value)
                && (!queryParams.FromDate.HasValue || order.CreatedAt >= queryParams.FromDate.Value)
                && (!queryParams.ToDate.HasValue || order.CreatedAt <= queryParams.ToDate.Value.Date.AddDays(1)))
        {
        }
    }
}