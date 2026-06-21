using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;

namespace PharmaBridge.Services.Specifications.BidSpec
{

    public class BidWithDetailsSpecification : BaseSpecifications<Bid, int>
    {
        public BidWithDetailsSpecification(int bidId)
            : base(b => b.Id == bidId)
        {
            ApplyIncludes();
        }

        private void ApplyIncludes()
        {
            
            AddInclude(b => b.PrescriptionRequest);

            // Needed for idempotency check bid.Order is not null
            AddInclude(b => b.Order);
        }
    }
}