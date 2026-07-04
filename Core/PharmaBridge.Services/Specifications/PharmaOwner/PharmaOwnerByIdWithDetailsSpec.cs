using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;

namespace PharmaBridge.Services.Specifications.PharmaOwners
{
    public class PharmaOwnerByIdWithDetailsSpec : BaseSpecifications<Domain.Models.User.PharmaOwner, string>
    {
        public PharmaOwnerByIdWithDetailsSpec(string pharmaOwnerId)
            : base(p => p.Id == pharmaOwnerId)
        {
            AddInclude(p => p.ApplicationUser);
            AddInclude(p => p.Pharmacy);
        }
    }
}
