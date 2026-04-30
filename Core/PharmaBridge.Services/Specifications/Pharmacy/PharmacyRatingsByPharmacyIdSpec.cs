using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.Common.Params.Pharmacy;

namespace PharmaBridge.Services.Specifications.Ratings
{
    public class PharmacyRatingsByPharmacyIdSpec : BaseSpecifications<PharmacyRating, int>
    {
        public PharmacyRatingsByPharmacyIdSpec(int pharmacyId)
            : base(r => r.PharmacyId == pharmacyId)
        {
            AddInclude(r => r.PatientProfile);
            AddInclude("PatientProfile.ApplicationUser");
            AddOrderBy(r => r.CreatedAt, true); // Most recent first
        }

        public PharmacyRatingsByPharmacyIdSpec(int pharmacyId, PharmacyRatingQueryParams queryParams)
            : base(r => r.PharmacyId == pharmacyId &&
                       (!queryParams.RatingValue.HasValue || r.RatingValue == queryParams.RatingValue.Value) &&
                       (string.IsNullOrEmpty(queryParams.Search) || (r.Comment != null && r.Comment.ToLower().Contains(queryParams.Search.ToLower()))))
        {
            AddInclude(r => r.PatientProfile);
            AddInclude("PatientProfile.ApplicationUser");
            AddOrderBy(r => r.CreatedAt, true); // Most recent first

            if (queryParams.PageSize > 0)
            {
                ApplyPaging(queryParams.PageSize, queryParams.PageIndex);
            }
        }
    }
}