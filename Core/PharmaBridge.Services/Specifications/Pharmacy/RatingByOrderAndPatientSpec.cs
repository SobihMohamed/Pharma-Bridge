using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;

namespace PharmaBridge.Services.Specifications.Ratings
{
    /// <summary>
    /// Checks if a rating already exists for a given Order AND Patient combination.
    /// More secure than filtering by OrderId alone — prevents information leakage
    /// about other patients' ratings.
    /// </summary>
    public class RatingByOrderAndPatientSpec : BaseSpecifications<PharmacyRating, int>
    {
        public RatingByOrderAndPatientSpec(int orderId, string patientProfileId)
            : base(r => r.OrderId == orderId && r.PatientProfileId == patientProfileId)
        {
        }
    }
}
