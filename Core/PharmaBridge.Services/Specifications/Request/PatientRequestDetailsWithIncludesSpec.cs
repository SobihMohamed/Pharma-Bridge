using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Request
{
    public class PatientRequestDetailsWithIncludesSpec : BaseSpecifications<PrescriptionRequestEntity,int>
    {
        public PatientRequestDetailsWithIncludesSpec(int requestId , string patientId) 
            :base(request => request.Id == requestId && request.PatientProfileId == patientId)
        {
            AddInclude(request => request.DeliveryAddress);
            AddInclude(request => request.Bids);

            var bidNavigate = $"{nameof(PrescriptionRequestEntity.Bids)}.{nameof(Bid.BidItems)}";
            AddInclude(bidNavigate);

            var pharamNavigate = $"{nameof(PrescriptionRequestEntity.Bids)}.{nameof(Bid.Pharmacy)}";
            AddInclude(pharamNavigate);
        }
    }
}
