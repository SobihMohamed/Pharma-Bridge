using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.PrescriptionRequest;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using PharmaBridge.Shared.DTOs.PharmaRequests;
using PharmaBridge.Shared.Common.Response;
using PharmaBridge.Shared.Common.Pagination;
using System.Security.Claims;

namespace PharmaBridge.Presentation.Controllers
{
    [Route("api/prescription-requests")]
    [Authorize(Roles = "Patient")]
    public class PrescriptionRequestController : AppBaseController
    {
        private readonly IPrescriptionRequestService _prescriptionRequestService;

        public PrescriptionRequestController(IPrescriptionRequestService prescriptionRequestService)
        {
            _prescriptionRequestService = prescriptionRequestService;
        }

        private Guid GetUserIdFromToken()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedAccessException("Invalid User ID in token.");
            return userId;
        }

        // 1. Create a new prescription request
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> CreateRequest([FromForm] CreatePrescriptionRequestDto createDto)
        {
            var userId = GetUserIdFromToken();
            var result = await _prescriptionRequestService.CreateRequestAsync(createDto, userId);
            return Created(result, "Prescription request created successfully");
        }

        // 2. Get all requests for the logged-in patient
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<object>>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> GetPatientRequests([FromQuery] PrescriptionRequestQueryParams queryParams)
        {
            var userId = GetUserIdFromToken();
            var result = await _prescriptionRequestService.GetPatientRequestsAsync(userId, queryParams);
            return Success(result, "Requests retrieved successfully");
        }

        // 3. Get specific request details
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> GetPatientRequestDetails(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await _prescriptionRequestService.GetPatientRequestDetailsAsync(id, userId);
            return Success(result, "Request details retrieved successfully");
        }

        // 4. Cancel Request
        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> CancelRequest(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await _prescriptionRequestService.CancelRequestAsync(id, userId);

            if (result)
                return Success("Request cancelled successfully");

            return BadRequestError("Failed to cancel request. It might already have accepted bids or is already closed.");
        }
    }
}