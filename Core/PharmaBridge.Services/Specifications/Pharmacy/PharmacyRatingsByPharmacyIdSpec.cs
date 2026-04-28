using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Models.UserAccess;

namespace PharmaBridge.Services.Specifications.Ratings
{
    public class PharmacyRatingsByPharmacyIdSpec : BaseSpecifications<PharmacyRating, int>
    {
        public PharmacyRatingsByPharmacyIdSpec(int pharmacyId)
            : base(r => r.PharmacyId == pharmacyId)
        {
        }
    }
}