using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;

namespace PharmaBridge.Services.Specifications.PharmaOwners
{
    public class PharmaOwnerByAppUserIdSpecification : BaseSpecifications<PharmaOwner, string>
    {
        public PharmaOwnerByAppUserIdSpecification(string applicationUserId)
            : base(owner => owner.ApplicationUserId == applicationUserId)
        {
        }
    }
}