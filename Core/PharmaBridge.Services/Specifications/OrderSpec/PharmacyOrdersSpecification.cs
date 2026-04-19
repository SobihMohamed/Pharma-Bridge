using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.Common.Params.Order;

namespace PharmaBridge.Services.Specifications.OrderSpec
{
    public class PharmacyOrdersSpecification : BaseSpecifications<Order, int>
    {
        public PharmacyOrdersSpecification(int pharmacyId, OrderQueryParams queryParams)
            : base(order => order.PharmacyId == pharmacyId
                && (!queryParams.Status.HasValue || order.OrderStatus == queryParams.Status.Value)
                && (!queryParams.FromDate.HasValue || order.CreatedAt >= queryParams.FromDate.Value)
                && (!queryParams.ToDate.HasValue || order.CreatedAt <= queryParams.ToDate.Value.Date.AddDays(1)))
        {
            AddInclude(o => o.Pharmacy);
            AddInclude(o => o.PatientProfile);

            // Most recent orders first
            AddOrderBy(o => o.CreatedAt, isDescending: true);

            // Pagination
            ApplyPaging(queryParams.PageSize, queryParams.PageIndex);
        }
    }
}