using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;

namespace PharmaBridge.Services.Specifications.Ratings
{
    public class OrderForRatingValidationSpec : BaseSpecifications<Order, int>
    {
        public OrderForRatingValidationSpec(int orderId, int pharmacyId, string patientId)
            : base(o => o.Id == orderId &&
                        o.PharmacyId == pharmacyId &&
                        o.PatientProfileId == patientId &&
                        o.OrderStatus == OrderStatus.Delivered)
        {
        }
    }
}