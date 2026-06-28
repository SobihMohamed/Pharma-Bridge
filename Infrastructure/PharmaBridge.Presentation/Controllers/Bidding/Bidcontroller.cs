using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Bidding;
using PharmaBridge.Presentation.Controllers;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Bid;
using PharmaBridge.Shared.DTOs.Bid;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Shared.Common.Response;
using System.Security.Claims;

namespace PharmaBridge.API.Controllers
{
    [Route("api/bids")] 
    [Authorize]
    public class BidController : AppBaseController
    {
        private readonly IBidService _bidService;

        public BidController(IBidService bidService)
        {
            _bidService = bidService;
        }

        [HttpPost("pharmacy/{pharmacyId:int}")]
        [Authorize(Roles = "PharmacyOwner")] 
        [ProducesResponseType(typeof(ApiResponse<BidDetailsDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> CreateBid(
            [FromRoute] int pharmacyId,
            [FromBody] CreateBidDto createBidDto)
        {
            var result = await _bidService.CreateBidAsync(createBidDto, pharmacyId);
            return Created(result, "Bid created successfully.");
        }

        [HttpGet("pharmacy/{pharmacyId:int}")]
        [Authorize(Roles = "PharmacyOwner")]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<BidDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPharmacyBids(
            [FromRoute] int pharmacyId,
            [FromQuery] BidQueryParams queryParams)
        {
            var result = await _bidService.GetPharmacyBidsAsync(pharmacyId, queryParams);
            return Success(result, "Pharmacy bids retrieved successfully.");
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<BidDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> GetAllPlatformBids(
            [FromQuery] BidQueryParams queryParams)
        {
            var result = await _bidService.GetAllPlatformBidsAsync(queryParams);
            return Success(result, "All platform bids retrieved successfully.");
        }

        [HttpGet("admin/{bidId:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<AdminBidDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAdminBidDetails([FromRoute] int bidId)
        {
            var result = await _bidService.GetAdminBidDetailsAsync(bidId);
            return Success(result, "Bid details retrieved successfully.");
        }

        // ================= Phase 2 ================= //

        [HttpPut("pharmacy/{pharmacyId:int}")]
        [Authorize(Roles = "PharmacyOwner")]
        [ProducesResponseType(typeof(ApiResponse<BidDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateBid(
            [FromRoute] int pharmacyId,
            [FromBody] UpdateBidDto updateBidDto)
        {
            var result = await _bidService.UpdateBidAsync(updateBidDto, pharmacyId);
            return Success(result, "Bid updated successfully.");
        }

        [HttpGet("request/{requestId:int}")]
        [Authorize(Roles = "Patient")] 
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<BidDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetBidsForRequest(
            [FromRoute] int requestId,
            [FromQuery] BidQueryParams queryParams)
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientId)) return UnauthorizedError();

            var result = await _bidService.GetBidsForRequestAsync(requestId, patientId, queryParams);
            return Success(result, "Bids for request retrieved successfully.");
        }

        [HttpPatch("{bidId:int}/status")]
        [Authorize(Roles = "Patient")] 
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RespondToBid(
            [FromRoute] int bidId,
            [FromQuery] BidStatus status)
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(patientId)) return UnauthorizedError();

            await _bidService.RespondToBidAsync(bidId, patientId, status);

            var message = status == BidStatus.Accepted
                ? "Bid accepted successfully."
                : "Bid rejected successfully.";

            return Success(true, message);
        }

        [HttpGet("{bidId:int}")]
        [ProducesResponseType(typeof(ApiResponse<BidDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetBidDetails([FromRoute] int bidId)
        {
            var result = await _bidService.GetBidDetailsAsync(bidId);
            return Success(result, "Bid details retrieved successfully.");
        }
    }
}