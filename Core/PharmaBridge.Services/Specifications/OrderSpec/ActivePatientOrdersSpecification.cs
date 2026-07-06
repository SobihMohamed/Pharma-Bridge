using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;

namespace PharmaBridge.Services.Specifications.OrderSpec
{
    public sealed class ActivePatientOrdersSpecification : BaseSpecifications<Order, int>
    {
        private static readonly OrderStatus[] ActiveStatuses =
        [
            OrderStatus.Pending,
            OrderStatus.Accepted,
            OrderStatus.Preparing,
            OrderStatus.InTransit
        ];

        public ActivePatientOrdersSpecification(string patientProfileId, int take = 5)
            : base(o =>
                !o.IsDeleted
                && o.PatientProfileId == patientProfileId
                && ActiveStatuses.Contains(o.OrderStatus))
        {
            AddInclude(o => o.Pharmacy);
            AddInclude("PatientProfile.ApplicationUser");
            AddOrderBy(o => o.CreatedAt, isDescending: true);
            ApplyPaging(take, 1);
        }
    }
}