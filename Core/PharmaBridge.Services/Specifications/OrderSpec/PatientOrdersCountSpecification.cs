using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.Common.Params.Order;

namespace PharmaBridge.Services.Specifications.OrderSpec
{
    public class PatientOrdersCountSpecification : BaseSpecifications<Order, int>
    {
        public PatientOrdersCountSpecification(string patientId, OrderQueryParams queryParams)
            : base(order =>
                order.PatientProfileId == patientId
                && (!queryParams.Status.HasValue || order.OrderStatus == queryParams.Status.Value)
                && (!queryParams.FromDate.HasValue || order.CreatedAt >= queryParams.FromDate.Value)
                && (!queryParams.ToDate.HasValue || order.CreatedAt <= queryParams.ToDate.Value.Date.AddDays(1)))
        {
        }
    }
}

