using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;

namespace PharmaBridge.Services.Specifications.AdminDashboard
{
    public sealed class AdminTotalPatientsCountSpec : BaseSpecifications<PatientProfile, string>
    {
        public AdminTotalPatientsCountSpec()
            : base(p => !p.IsDeleted) { }
    }

    public sealed class AdminPatientsByPeriodCountSpec : BaseSpecifications<PatientProfile, string>
    {
        public AdminPatientsByPeriodCountSpec(DateTime from, DateTime to)
            : base(p => !p.IsDeleted && p.CreatedAt >= from && p.CreatedAt < to) { }
    }
}