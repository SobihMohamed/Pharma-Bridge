using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Bidding;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Bid;
using PharmaBridge.Shared.DTOs.Bid;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System.Security.Claims;

namespace PharmaBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BidController : ControllerBase
    {
        private readonly IBidService _bidService;

        public BidController(IBidService bidService)
        {
            _bidService = bidService;
        }


        [HttpPost("pharmacy/{pharmacyId:int}")]
        public async Task<ActionResult<BidDetailsDto>> CreateBid(
            [FromRoute] int pharmacyId,
            [FromBody] CreateBidDto createBidDto)
        {
            var result = await _bidService.CreateBidAsync(createBidDto, pharmacyId);

            return CreatedAtAction(
                nameof(GetAdminBidDetails),
                new { bidId = result.Id },
                result);
        }

        [HttpGet("pharmacy/{pharmacyId:int}")]
        public async Task<ActionResult<PaginationResponse<BidDto>>> GetPharmacyBids(
            [FromRoute] int pharmacyId,
            [FromQuery] BidQueryParams queryParams)
        {
            var result = await _bidService.GetPharmacyBidsAsync(pharmacyId, queryParams);
            return Ok(result);
        }

        
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginationResponse<BidDto>>> GetAllPlatformBids(
            [FromQuery] BidQueryParams queryParams)
        {
            var result = await _bidService.GetAllPlatformBidsAsync(queryParams);
            return Ok(result);
        }

        [HttpGet("admin/{bidId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminBidDetailsDto>> GetAdminBidDetails(
            [FromRoute] int bidId)
        {
            var result = await _bidService.GetAdminBidDetailsAsync(bidId);
            return Ok(result);
        }

        // Phase 2

        [HttpPut("pharmacy/{pharmacyId:int}")]
        public async Task<ActionResult<BidDetailsDto>> UpdateBid(
            [FromRoute] int pharmacyId,
            [FromBody] UpdateBidDto updateBidDto)
        {
            var result = await _bidService.UpdateBidAsync(updateBidDto, pharmacyId);
            return Ok(result);
        }

        [HttpGet("request/{requestId:int}")]
        public async Task<ActionResult<PaginationResponse<BidDto>>> GetBidsForRequest(
            [FromRoute] int requestId,
            [FromQuery] BidQueryParams queryParams)
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _bidService.GetBidsForRequestAsync(
                requestId,
                patientId,
                queryParams);

            return Ok(result);
        }


        [HttpPatch("{bidId:int}/status")]
        public async Task<IActionResult> RespondToBid(
            [FromRoute] int bidId,
            [FromQuery] BidStatus status)
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _bidService.RespondToBidAsync(bidId, patientId, status);

            var message = status == BidStatus.Accepted
                ? "Bid accepted successfully."
                : "Bid rejected successfully.";

            return Ok(new { message });
        }


        [HttpGet("{bidId:int}")]
        public async Task<ActionResult<BidDetailsDto>> GetBidDetails(
            [FromRoute] int bidId)
        {
            var result = await _bidService.GetBidDetailsAsync(bidId);
            return Ok(result);
        }



    }
}