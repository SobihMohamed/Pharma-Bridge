using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.Common.Params.Pharmacy;
using Microsoft.EntityFrameworkCore;

namespace PharmaBridge.Services.Specifications.Pharmacy
{
    public class PharmacyWithFiltersSpec : BaseSpecifications<Domain.Models.Pharma_Requests.Pharmacy, int>
    {
        public PharmacyWithFiltersSpec(PharmacyQueryParams queryParams, bool isCountSpec = false)
            : base(p =>
                (!queryParams.Status.HasValue || p.Status == queryParams.Status.Value) &&
                (string.IsNullOrEmpty(queryParams.Search) ||
                 EF.Functions.Like(p.PharmacyName, $"%{queryParams.Search}%") ||
                 (p.PharmaOwner != null && p.PharmaOwner.ApplicationUser != null &&
                  (
                      EF.Functions.Like(p.PharmaOwner.ApplicationUser.FullName, $"%{queryParams.Search}%") ||
                      EF.Functions.Like(p.PharmaOwner.ApplicationUser.Email!, $"%{queryParams.Search}%") ||
                      (p.PharmaOwner.ApplicationUser.PhoneNumber != null &&
                       EF.Functions.Like(p.PharmaOwner.ApplicationUser.PhoneNumber, $"%{queryParams.Search}%"))
                  )) ||
                 (p.ContactPhone != null && EF.Functions.Like(p.ContactPhone, $"%{queryParams.Search}%")))
            )
        {
            if (!isCountSpec)
            {
                AddInclude(p => p.PharmaOwner);
                var nestedInclude = nameof(PharmaOwner) + "." + nameof(PharmaOwner.ApplicationUser);
                AddInclude(nestedInclude);
                AddOrderBy(p => p.CreatedAt, isDescending: true);
                ApplyPaging(queryParams.PageSize, queryParams.PageIndex);
            }
        }
    }
}
