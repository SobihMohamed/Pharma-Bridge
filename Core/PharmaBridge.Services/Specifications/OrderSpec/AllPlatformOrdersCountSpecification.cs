using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.Common.Params.Order;

namespace PharmaBridge.Services.Specifications.OrderSpec
{
    public class AllPlatformOrdersCountSpecification : BaseSpecifications<Order, int>
    {
        public AllPlatformOrdersCountSpecification(OrderQueryParams queryParams)
            : base(order =>
                (!queryParams.Status.HasValue || order.OrderStatus == queryParams.Status.Value)
                && (!queryParams.PharmacyId.HasValue || order.PharmacyId == queryParams.PharmacyId.Value)
                && (string.IsNullOrEmpty(queryParams.PatientId) || order.PatientProfileId.ToLower() == queryParams.PatientId) && (!queryParams.FromDate.HasValue || order.CreatedAt >= queryParams.FromDate.Value)
                && (!queryParams.ToDate.HasValue || order.CreatedAt <= queryParams.ToDate.Value.Date.AddDays(1)) && (string.IsNullOrEmpty(queryParams.Search)
            || order.Pharmacy.PharmacyName.ToLower().Contains(queryParams.Search)
            || order.PatientProfile.ApplicationUser.FullName.ToLower().Contains(queryParams.Search)))
        {
        }
    }
}
