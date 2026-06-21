using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;

namespace PharmaBridge.Services.Specifications.Ratings
{
    public class RatingByOrderIdSpec : BaseSpecifications<PharmacyRating, int>
    {
        public RatingByOrderIdSpec(int orderId)
            : base(r => r.OrderId == orderId)
        {
        }
    }
}