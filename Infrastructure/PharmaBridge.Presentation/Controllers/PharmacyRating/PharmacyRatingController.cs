using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Pharmacy;
using PharmaBridge.Shared.Common.Response; 
using PharmaBridge.Shared.DTOs.PharmacyRating;
using System.Security.Claims;

namespace PharmaBridge.Presentation.Controllers
{
    [Route("api/pharmacy-rating")]
    [ApiController]
    public class PharmacyRatingController(IPharmacyRatingService pharmacyRatingService) : AppBaseController
    {
        // 1. Submit Rating
        [HttpPost("submit")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
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

        // 2. Get Pharmacy Reviews
        [HttpGet("pharmacy/{pharmacyId}")]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<PharmacyRatingDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPharmacyReviews(int pharmacyId, [FromQuery] PharmacyRatingQueryParams queryParams)
        {
            var reviews = await pharmacyRatingService.GetPharmacyReviewsAsync(pharmacyId, queryParams);

            return Success(reviews, "Reviews retrieved successfully.");
        }
    }
}