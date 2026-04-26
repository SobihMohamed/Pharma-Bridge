
using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;

namespace PharmaBridge.Services.Specifications.Bidding
{
    
    public sealed class DuplicateBidCheckSpecification : BaseSpecifications<Bid, int>
    {
        public DuplicateBidCheckSpecification(int pharmacyId, int prescriptionRequestId)
            : base(b =>
                !b.IsDeleted
                && b.PharmacyId == pharmacyId
                && b.PrescriptionRequestId == prescriptionRequestId)
        {
        }
    }
}