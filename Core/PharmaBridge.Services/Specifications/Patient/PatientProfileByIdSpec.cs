using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;

namespace PharmaBridge.Services.Specifications.Patient
{
    internal class PatientProfileByIdSpec : BaseSpecifications<PatientProfile, string>
    {
        public PatientProfileByIdSpec(string id)
            : base(p => p.Id == id)
        {
            AddInclude(p => p.ApplicationUser);
        }
    }
}