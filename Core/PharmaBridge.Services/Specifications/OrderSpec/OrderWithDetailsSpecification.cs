using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Domain.Models.UserAccess;

namespace PharmaBridge.Services.Specifications.OrderSpec
{
    public class OrderWithDetailsSpecification : BaseSpecifications<Order, int>
    {
        public OrderWithDetailsSpecification(int orderId)
            : base(o => o.Id == orderId)
        {
            ApplyIncludes();
        }

        private void ApplyIncludes()
        {
            AddInclude(o => o.Pharmacy);
            AddInclude(o => o.PatientAddress);
            AddInclude(o => o.PrescriptionRequest);
            AddInclude(o => o.PatientProfile);

            // (needed for FullName and PhoneNumber)
            AddInclude(nameof(Order.PatientProfile) + "." + nameof(PatientProfile.ApplicationUser));

            // Bid, BidItems
            AddInclude(nameof(Order.Bid) + "." + nameof(Bid.BidItems));

            AddInclude(o => o.PharmacyRating!);
        }
    }
}