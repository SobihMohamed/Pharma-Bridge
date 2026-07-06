using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;

namespace PharmaBridge.Services.Specifications.Request
{
    public sealed class LatestPatientRequestsSpecification : BaseSpecifications<PrescriptionRequestEntity, int>
    {
        public LatestPatientRequestsSpecification(string patientProfileId, int take = 5)
            : base(r => !r.IsDeleted && r.PatientProfileId == patientProfileId)
        {
            AddInclude(r => r.Bids);
            AddInclude(r => r.DeliveryAddress);
            AddOrderBy(r => r.CreatedAt, isDescending: true);
            ApplyPaging(take, 1);
        }
    }
}