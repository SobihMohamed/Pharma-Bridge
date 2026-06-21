
using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;

namespace PharmaBridge.Services.Specifications.Bidding
{
    public sealed class BidWithDetailsSpecification : BaseSpecifications<Bid, int>
    {
        public BidWithDetailsSpecification(int bidId)
            : base(b => !b.IsDeleted && b.Id == bidId)
        {
            AddInclude(b => b.BidItems);
            AddInclude(b => b.Pharmacy);
            //AddInclude(b => b.Order);  // needed for nullable OrderId
            AddInclude(b => b.PrescriptionRequest);
        }
    }
}