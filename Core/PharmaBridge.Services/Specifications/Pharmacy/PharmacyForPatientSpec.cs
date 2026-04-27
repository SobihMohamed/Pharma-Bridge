using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Services.Specifications.Pharmacy
{
    public class PharmacyForPatientSpec : BaseSpecifications<Domain.Models.Pharma_Requests.Pharmacy, int>
    {
        public PharmacyForPatientSpec(int pharmacyId)
            : base(p => p.Id == pharmacyId && p.Status == PharmacyStatus.Active)
        {
            AddInclude(p => p.PharmaOwner);
            var nestedInclude = nameof(PharmaOwner) + "." + nameof(Domain.Models.User.PharmaOwner.ApplicationUser);
            AddInclude(nestedInclude);
        }
    }
}
