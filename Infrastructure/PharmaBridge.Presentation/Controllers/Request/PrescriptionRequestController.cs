using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices; // غير ده للـ namespace بتاعك
using PharmaBridge.Abstraction.IServices.PrescriptionRequest;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using PharmaBridge.Shared.DTOs; // غير ده للـ namespace بتاعك
using PharmaBridge.Shared.DTOs.PharmaRequests;
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

        // Helper Method: to extract the patient ID from the JWT token
        private Guid GetUserIdFromToken()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedAccessException("Invalid User ID in token.");
            return userId;
        }

        // 1. Create a new prescription request
        [HttpPost]
        public async Task<ActionResult> CreateRequest([FromForm] CreatePrescriptionRequestDto createDto)
        {
            var userId = GetUserIdFromToken();
            var result = await _prescriptionRequestService.CreateRequestAsync(createDto, userId);
            return Created(result, "Prescription request created successfully");
        }

        // 2. Get all requests for the logged-in patient (with pagination)
        [HttpGet]
        public async Task<ActionResult> GetPatientRequests([FromQuery] PrescriptionRequestQueryParams queryParams)
        {
            var userId = GetUserIdFromToken();
            var result = await _prescriptionRequestService.GetPatientRequestsAsync(userId, queryParams);
            return Success(result, "Requests retrieved successfully");
        }

        // 3. Get specific request details
        [HttpGet("{id}")]
        public async Task<ActionResult> GetPatientRequestDetails(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await _prescriptionRequestService.GetPatientRequestDetailsAsync(id, userId);
            return Success(result, "Request details retrieved successfully");
        }

        [HttpPatch("{id}/cancel")]
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