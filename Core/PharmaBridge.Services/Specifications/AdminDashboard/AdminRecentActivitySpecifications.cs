using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.UserAccess;

namespace PharmaBridge.Services.Specifications.AdminDashboard
{
    public sealed class AdminRecentOrdersSpec : BaseSpecifications<Order, int>
    {
        public AdminRecentOrdersSpec(int take = 4)
            : base(o => !o.IsDeleted)
        {
            AddInclude(o => o.Pharmacy);
            AddInclude("PatientProfile.ApplicationUser");
            AddOrderBy(o => o.CreatedAt, isDescending: true);
            ApplyPaging(take, 1);
        }
    }

    public sealed class AdminRecentBidsSpec : BaseSpecifications<Bid, int>
    {
        public AdminRecentBidsSpec(int take = 4)
            : base(b => !b.IsDeleted)
        {
            AddInclude(b => b.Pharmacy);
            AddOrderBy(b => b.SubmittedAt, isDescending: true);
            ApplyPaging(take, 1);
        }
    }

    public sealed class AdminRecentRequestsSpec : BaseSpecifications<PrescriptionRequestEntity, int>
    {
        public AdminRecentRequestsSpec(int take = 4)
            : base(r => !r.IsDeleted)
        {
            AddInclude("PatientProfile.ApplicationUser");
            AddOrderBy(r => r.CreatedAt, isDescending: true);
            ApplyPaging(take, 1);
        }
    }
}