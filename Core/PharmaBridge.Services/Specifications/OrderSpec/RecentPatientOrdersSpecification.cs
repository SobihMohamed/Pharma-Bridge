using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.UserAccess;

namespace PharmaBridge.Services.Specifications.OrderSpec
{
    public sealed class RecentPatientOrdersSpecification : BaseSpecifications<Order, int>
    {
        public RecentPatientOrdersSpecification(string patientProfileId, int take = 5)
            : base(o =>
                !o.IsDeleted
                && o.PatientProfileId == patientProfileId) 
        {
            AddInclude(o => o.Pharmacy);
            AddInclude("PatientProfile.ApplicationUser");
            AddOrderBy(o => o.CreatedAt, isDescending: true); 
            ApplyPaging(take, 1); 
        }
    }
}