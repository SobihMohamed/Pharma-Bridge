using PharmaBridge.Shared.Common.Params.Bid;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.DTOs.Bid;

namespace PharmaBridge.Abstraction.IServices.Bidding
{
    // This interface defines the contract for the Bidding workflow.
    // Handles pharmacies submitting bids (prices & items) for prescription requests 
    // and patients reviewing, accepting, or rejecting these bids.
    public interface IBidService
    {
        // --- Pharmacy (Provider) Operations ---

        // Pharmacy submits a bid containing multiple BidItems (prices, notes) for a specific request.
        // Triggers PrescriptionRequest status update to 'HasBids'.
        Task<BidDetailsDto> CreateBidAsync(CreateBidDto createBidDto, int pharmacyId);

        // Pharmacy can update their bid (e.g., adjust price) BEFORE the patient accepts it.
        //Task<BidDetailsDto> UpdateBidAsync(UpdateBidDto updateBidDto, Guid pharmacyId);

        // Pharmacy can view all their submitted bids with filtering options.
        Task<PaginationResponse<BidDto>> GetPharmacyBidsAsync(int pharmacyId, BidQueryParams queryParams);


        // --- Patient (Client) Operations ---

        // Patient views all received bids for a specific prescription request.
        //Task<Pagination<BidDto>> GetBidsForRequestAsync(Guid requestId, Guid patientId, BidQueryParams queryParams);

        // Patient accepts a specific bid. 
        // CRITICAL: Closes the PrescriptionRequest, rejects other bids, and triggers Order creation!
        //Task<bool> AcceptBidAsync(Guid bidId, Guid patientId);

        // Patient manually rejects a specific bid.
        //Task<bool> RejectBidAsync(Guid bidId, Guid patientId);


        // --- Shared Operations (Patient & Pharmacy) ---

        // View full details of a specific bid including the List of BidItemDto.
        //Task<BidDetailsDto> GetBidDetailsAsync(Guid bidId);


        // --- Admin Operations ---

        // Admin views all bids across the platform for monitoring.
        Task<PaginationResponse<BidDto>> GetAllPlatformBidsAsync(BidQueryParams queryParams);

        // Admin views sensitive/full details of a bid using the specific Admin DTO.
        Task<AdminBidDetailsDto> GetAdminBidDetailsAsync(int bidId);
    }
}

/*
// --- Query Params Classes (To be placed in Shared/Common/params/Bidding folder) ---

public class BidQueryParams : BaseQueryParams
{
    // e.g., Pending, Accepted, Rejected, Expired
    public BidStatus? Status { get; set; } 
    
    // To filter bids related to a specific Prescription Request
    public Guid? PrescriptionRequestId { get; set; } 
    
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
*/