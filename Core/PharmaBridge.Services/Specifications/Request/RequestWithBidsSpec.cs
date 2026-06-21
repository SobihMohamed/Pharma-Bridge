using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;

namespace PharmaBridge.Services.Specifications.Request
{
    public class RequestWithBidsSpec : BaseSpecifications<PrescriptionRequestEntity, int>
    {
        public RequestWithBidsSpec(int requestId) 
            : base(r => r.Id == requestId)
        {
            AddInclude(r => r.Bids);   
        }
    }
}
