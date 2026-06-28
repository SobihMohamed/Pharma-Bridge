using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.Common.Params.PharmaOwner;
using Microsoft.EntityFrameworkCore;

namespace PharmaBridge.Services.Specifications.PharmaOwners
{
    public class PharmaOwnerWithFiltersSpec : BaseSpecifications<Domain.Models.User.PharmaOwner, string>
    {
        public PharmaOwnerWithFiltersSpec(PharmaOwnerQueryParams queryParams)
            : base(p =>
                string.IsNullOrEmpty(queryParams.Search) ||
                (p.ApplicationUser != null &&
                 (
                     EF.Functions.Like(p.ApplicationUser.FullName, $"%{queryParams.Search}%") ||
                     EF.Functions.Like(p.ApplicationUser.Email!, $"%{queryParams.Search}%") ||
                     (p.ApplicationUser.PhoneNumber != null &&
                      EF.Functions.Like(p.ApplicationUser.PhoneNumber, $"%{queryParams.Search}%"))
                 ))
            )
        {
            AddInclude(p => p.ApplicationUser);
            AddOrderBy(p => p.CreatedAt, isDescending: true);
            ApplyPaging(queryParams.PageSize, queryParams.PageIndex);
        }

        public PharmaOwnerWithFiltersSpec(string? search)
            : base(p =>
                string.IsNullOrEmpty(search) ||
                (p.ApplicationUser != null &&
                 (
                     EF.Functions.Like(p.ApplicationUser.FullName, $"%{search}%") ||
                     EF.Functions.Like(p.ApplicationUser.Email!, $"%{search}%") ||
                     (p.ApplicationUser.PhoneNumber != null &&
                      EF.Functions.Like(p.ApplicationUser.PhoneNumber, $"%{search}%"))
                 ))
            )
        {
        }
    }
}
