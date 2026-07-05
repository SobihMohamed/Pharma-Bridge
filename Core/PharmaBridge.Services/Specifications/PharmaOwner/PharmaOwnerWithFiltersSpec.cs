using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.Common.Params.PharmaOwner;
using Microsoft.EntityFrameworkCore;

namespace PharmaBridge.Services.Specifications.PharmaOwners
{
    public class PharmaOwnerWithFiltersSpec : BaseSpecifications<Domain.Models.User.PharmaOwner, string>
    {
        public PharmaOwnerWithFiltersSpec(PharmaOwnerQueryParams queryParams, bool isCountSpec = false)
            : base(p =>
                (!queryParams.Status.HasValue || p.Status == queryParams.Status.Value) &&
                (string.IsNullOrEmpty(queryParams.Search) ||
                 (p.ApplicationUser != null &&
                  (
                      EF.Functions.Like(p.ApplicationUser.FullName, $"%{queryParams.Search}%") ||
                      EF.Functions.Like(p.ApplicationUser.Email!, $"%{queryParams.Search}%") ||
                      (p.ApplicationUser.PhoneNumber != null &&
                       EF.Functions.Like(p.ApplicationUser.PhoneNumber, $"%{queryParams.Search}%"))
                  )))
            )
        {
            if (!isCountSpec)
            {
                AddInclude(p => p.ApplicationUser);
                AddOrderBy(p => p.CreatedAt, isDescending: true);
                ApplyPaging(queryParams.PageSize, queryParams.PageIndex);
            }
        }
    }
}
