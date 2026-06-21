using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;


namespace PharmaBridge.Services.Specifications.Request
{
    public class RequestWithAddressSpec : BaseSpecifications<PrescriptionRequestEntity,int>
    {
        public RequestWithAddressSpec(int reqId)
            :base(req => req.Id == reqId) 
        {
            AddInclude(r => r.DeliveryAddress);
        }
    }
}
