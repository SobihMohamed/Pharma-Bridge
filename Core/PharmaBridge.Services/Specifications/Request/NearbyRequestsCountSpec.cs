using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Services.Specifications.Request
{
    public class NearbyRequestsCountSpec : BaseSpecifications<PrescriptionRequestEntity, int>
    {
        public NearbyRequestsCountSpec(Domain.Models.Pharma_Requests.Pharmacy pharmacy, PrescriptionRequestQueryParams queryParams, decimal radiusInDegrees)
            : base(r =>
                // filter by status (Pending or HasBids) only 
                (r.Status == PrescriptionStatus.Pending || r.Status == PrescriptionStatus.HasBids) &&

                // if the search term is provided, filter by medicine name (case-insensitive)
                (string.IsNullOrEmpty(queryParams.Search) ||
                 (!string.IsNullOrEmpty(r.MedicineName) && r.MedicineName.ToLower().Contains(queryParams.Search))) &&

                // filter by delivery address proximity to the pharmacy location using boxing method for performance (only if the request has a delivery address)
                (r.DeliveryAddress != null &&
                 // get the requests that the north-south distance is within the radius
                 r.DeliveryAddress.Latitude >= pharmacy.Latitude - radiusInDegrees && 
                 r.DeliveryAddress.Latitude <= pharmacy.Latitude + radiusInDegrees &&
                 // get the requests that the east-west distance is within the radius
                 r.DeliveryAddress.Longitude >= pharmacy.Longitude - radiusInDegrees &&
                 r.DeliveryAddress.Longitude <= pharmacy.Longitude + radiusInDegrees)
            )
        {
        }
    }
}