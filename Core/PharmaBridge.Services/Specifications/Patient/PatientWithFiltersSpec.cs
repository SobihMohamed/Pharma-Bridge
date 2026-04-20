using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.Common.Params.Patient;

namespace PharmaBridge.Services.Specifications.Patient
{
    public class PatientWithFiltersSpec : BaseSpecifications<PatientProfile, string>
    {
        public PatientWithFiltersSpec(PatientQueryParams queryParams)
            : base(p =>
                string.IsNullOrEmpty(queryParams.Search) ||
                p.ApplicationUser!.FullName.ToLower().Contains(queryParams.Search) ||
                p.ApplicationUser.Email!.ToLower().Contains(queryParams.Search) ||
                p.ApplicationUser.PhoneNumber!.Contains(queryParams.Search)
            )
        {
            AddInclude(p => p.ApplicationUser!);
            AddOrderBy(p => p.CreatedAt, isDescending: true);
            ApplyPaging(queryParams.PageSize, queryParams.PageIndex);
        }

        public PatientWithFiltersSpec(string? search)
            : base(p =>
                string.IsNullOrEmpty(search) ||
                p.ApplicationUser!.FullName.ToLower().Contains(search) ||
                p.ApplicationUser.Email!.ToLower().Contains(search) ||
                p.ApplicationUser.PhoneNumber!.Contains(search)
            )
        {
        }
    }
}