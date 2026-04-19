using PharmaBridge.Shared.DTOs.Bid;
using System;

namespace PharmaBridge.Shared.DTOs.PharmaRequests
{
    // Inherits everything from the List DTO (Id, ImageUrl, MedicineName, Status, BidsCount, DeliveryArea, etc.)
    public class PrescriptionRequestDetailsDto : PrescriptionRequestDto
    {
        // to show the all bids of the request to the patient 
        public List<BidDto> Bids { get; set; } = new();
    }
}
