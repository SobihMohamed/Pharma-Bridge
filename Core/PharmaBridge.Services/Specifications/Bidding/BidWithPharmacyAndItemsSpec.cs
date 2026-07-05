
using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;

namespace PharmaBridge.Services.Specifications.Bidding
{
    public sealed class BidWithPharmacyAndItemsSpec : BaseSpecifications<Bid, int>
    {
        public BidWithPharmacyAndItemsSpec(int bidId)
            : base(b => !b.IsDeleted && b.Id == bidId)
        {
            AddInclude(b => b.BidItems);
            AddInclude(b => b.Pharmacy);
            var pharmacyOwner = nameof(Bid.Pharmacy) + "." + nameof(Bid.Pharmacy.PharmaOwner);
            AddInclude(pharmacyOwner);
            //AddInclude(b => b.Order);  // needed for nullable OrderId
            AddInclude(b => b.PrescriptionRequest);
        }
    }
}