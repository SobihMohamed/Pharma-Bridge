using PharmaBridge.Shared.DTOs.PharmacyRating;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Pharmacy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PharmaBridge.Abstraction.IServices.Pharmacy
{
    public interface IPharmacyRatingService
    {
        // =========================================================================
        // --- Patient (Client) Operations ---
        // =========================================================================

        // Patient submits a rating (1-5 stars) and an optional comment.
        // Validation needed inside: Check if Order is Completed and belongs to this Patient or not.
        Task<bool> SubmitRatingAsync(CreatePharmacyRatingDto createRatingDto, string patientId);


        // =========================================================================
        // --- Shared Operations (Patient, Pharmacy, Admin) ---
        // =========================================================================

        // Get paginated list of reviews for a specific pharmacy (To display on their profile).
        Task<PaginationResponse<PharmacyRatingDto>> GetPharmacyReviewsAsync(int pharmacyId, PharmacyRatingQueryParams queryParams);
    }
}