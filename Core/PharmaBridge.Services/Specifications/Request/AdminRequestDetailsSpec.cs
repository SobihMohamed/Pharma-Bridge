using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Request
{
    public class AdminRequestDetailsSpec : BaseSpecifications<PrescriptionRequestEntity, int>
    {
        public AdminRequestDetailsSpec(int requestId) : base(r => r.Id == requestId)
        {
            AddInclude(r => r.DeliveryAddress);
            AddInclude(r => r.PatientProfile);
            var UserNavigate = $"{nameof(PrescriptionRequestEntity.PatientProfile)}.{nameof(PatientProfile.ApplicationUser)}";
            AddInclude(UserNavigate);

            AddInclude(r => r.PrescriptionRequestHistorys);

            AddInclude(r => r.Bids);
            var pharamNavigate = $"{nameof(PrescriptionRequestEntity.Bids)}.{nameof(Bid.Pharmacy)}";
            AddInclude(pharamNavigate);
           
            var bidItemsNavigate = $"{nameof(PrescriptionRequestEntity.Bids)}.{nameof(Bid.BidItems)}";
            AddInclude(bidItemsNavigate);
        }
    }
}
