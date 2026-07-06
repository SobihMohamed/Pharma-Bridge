using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;

namespace PharmaBridge.Services.Specifications.Request
{
    public class NearbyRequestsCountSpec : BaseSpecifications<PrescriptionRequestEntity, int>
    {
        public NearbyRequestsCountSpec(Domain.Models.Pharma_Requests.Pharmacy pharmacy, PrescriptionRequestQueryParams queryParams, decimal radiusInDegrees)
            : base(r =>
                // 1. Status Filter & Expiration Check:
                (!queryParams.Status.HasValue ?
                    (r.ExpiresAt > DateTime.UtcNow && (r.Status == PrescriptionStatus.Pending || r.Status == PrescriptionStatus.HasBids)) :

                (queryParams.Status == PrescriptionStatus.Pending || queryParams.Status == PrescriptionStatus.HasBids) ?
                    (r.ExpiresAt > DateTime.UtcNow && r.Status == queryParams.Status) :

                    r.Status == queryParams.Status) &&

                // 2. Search Filter:
                (string.IsNullOrEmpty(queryParams.Search) ||
                 (!string.IsNullOrEmpty(r.MedicineName) && r.MedicineName.ToLower().Contains(queryParams.Search.ToLower()))) &&

                // 3. Date Filters:
                (!queryParams.FromDate.HasValue || r.CreatedAt >= queryParams.FromDate) &&
                (!queryParams.ToDate.HasValue || r.CreatedAt <= queryParams.ToDate) &&

                // 4. Patient Filter:
                (string.IsNullOrEmpty(queryParams.PatientId) || r.PatientProfile.ApplicationUserId == queryParams.PatientId) &&

                // 5. Radius Filter:
                (r.DeliveryAddress != null &&
                 r.DeliveryAddress.Latitude >= pharmacy.Latitude - radiusInDegrees &&
                 r.DeliveryAddress.Latitude <= pharmacy.Latitude + radiusInDegrees &&
                 r.DeliveryAddress.Longitude >= pharmacy.Longitude - radiusInDegrees &&
                 r.DeliveryAddress.Longitude <= pharmacy.Longitude + radiusInDegrees)
            )
        {
        }
    }
}