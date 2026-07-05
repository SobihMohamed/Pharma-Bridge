using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.Common.Params.Order;

namespace PharmaBridge.Services.Specifications.OrderSpec
{
    public class PatientOrdersSpecification : BaseSpecifications<Order, int>
    {
        public PatientOrdersSpecification(string patientId, OrderQueryParams queryParams)
            : base(order =>
                order.PatientProfileId == patientId
                && (!queryParams.Status.HasValue || order.OrderStatus == queryParams.Status.Value)
                && (!queryParams.FromDate.HasValue || order.CreatedAt >= queryParams.FromDate.Value)
                && (!queryParams.ToDate.HasValue || order.CreatedAt <= queryParams.ToDate.Value.Date.AddDays(1)))
        {
            AddInclude(o => o.Pharmacy);
            AddInclude($"{nameof(Order.PatientProfile)}.{nameof(PatientProfile.ApplicationUser)}");

            AddOrderBy(o => o.CreatedAt, isDescending: true);
            ApplyPaging(queryParams.PageSize, queryParams.PageIndex);
        }
    }
}
