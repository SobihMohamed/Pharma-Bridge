using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;

namespace PharmaBridge.Services.Specifications.Request
{
    public sealed class NearbyRequestsSpec : NearbyRequestsCountSpec
    {
        public NearbyRequestsSpec(Domain.Models.Pharma_Requests.Pharmacy pharmacy, PrescriptionRequestQueryParams queryParams, decimal radiusInDegrees)
            : base(pharmacy, queryParams, radiusInDegrees)
        {
            AddInclude(r => r.DeliveryAddress);

            // order by distance to the pharmacy using the Pythagorean formula approximation
            // (without the actual square root for performance, since we only need relative distances for ordering)
            AddOrderBy(r =>
                (r.DeliveryAddress.Latitude - pharmacy.Latitude) * (r.DeliveryAddress.Latitude - pharmacy.Latitude) +
                (r.DeliveryAddress.Longitude - pharmacy.Longitude) * (r.DeliveryAddress.Longitude - pharmacy.Longitude)
            );

            ApplyPaging(queryParams.PageSize, queryParams.PageIndex);
        }
    }
}