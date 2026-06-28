using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;

namespace PharmaBridge.Services.Specifications.PharmaOwners
{
    internal class PharmaOwnerProfileWithDetailsSpec : BaseSpecifications<Domain.Models.User.PharmaOwner, string>
    {
        public PharmaOwnerProfileWithDetailsSpec(string applicationUserId)
            : base(p => p.ApplicationUserId == applicationUserId)
        {
            AddInclude(p => p.ApplicationUser);
            AddInclude(p => p.Pharmacy);
        }
    }
}
