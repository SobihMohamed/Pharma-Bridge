using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Services.Specifications.AdminDashboard
{
    public sealed class AdminActiveBidsCountSpec : BaseSpecifications<Bid, int>
    {
        public AdminActiveBidsCountSpec()
            : base(b => !b.IsDeleted && b.Status == BidStatus.Pending) { }
    }

    public sealed class AdminExpiringRequestsWithBidsCountSpec : BaseSpecifications<PrescriptionRequestEntity, int>
    {
        public AdminExpiringRequestsWithBidsCountSpec(DateTime expiresWithin)
            : base(r =>
                !r.IsDeleted
                && r.Status == PrescriptionStatus.HasBids
                && r.ExpiresAt > DateTime.UtcNow
                && r.ExpiresAt <= expiresWithin)
        { }
    }
}