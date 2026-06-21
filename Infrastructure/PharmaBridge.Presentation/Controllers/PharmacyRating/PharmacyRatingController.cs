using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Shared.Common.Params.Pharmacy;
using PharmaBridge.Shared.DTOs.PharmacyRating;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PharmaBridge.Presentation.Controllers
{
    public class PharmacyRatingController(IPharmacyRatingService pharmacyRatingService) : AppBaseController
    {
        [HttpPost("submit")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> SubmitRating([FromBody] CreatePharmacyRatingDto dto)
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(patientId))
                return UnauthorizedError("User is not authorized.");

            var result = await pharmacyRatingService.SubmitRatingAsync(dto, patientId);

            if (result)
                return Success("Rating submitted successfully!");

            return BadRequestError("Failed to submit rating.");
        }

        [HttpGet("pharmacy/{pharmacyId}")]
        public async Task<IActionResult> GetPharmacyReviews(int pharmacyId, [FromQuery] PharmacyRatingQueryParams queryParams)
        {
            var reviews = await pharmacyRatingService.GetPharmacyReviewsAsync(pharmacyId, queryParams);

            return Success(reviews, "Reviews retrieved successfully.");
        }
    }
}