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
        private Guid GetPatientIdFromToken()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var patientId))
                throw new UnauthorizedAccessException("Invalid Patient ID in token.");
            return patientId;
        }

        // 1. Create a new prescription request
        [HttpPost]
        public async Task<ActionResult> CreateRequest([FromBody] CreatePrescriptionRequestDto createDto)
        {
            var patientId = GetPatientIdFromToken();
            var result = await _prescriptionRequestService.CreateRequestAsync(createDto, patientId);
            return Created(result, "Prescription request created successfully");
        }

        // 2. Get all requests for the logged-in patient (with pagination)
        [HttpGet]
        public async Task<ActionResult> GetPatientRequests([FromQuery] PrescriptionRequestQueryParams queryParams)
        {
            var patientId = GetPatientIdFromToken();
            var result = await _prescriptionRequestService.GetPatientRequestsAsync(patientId, queryParams);
            return Success(result, "Requests retrieved successfully");
        }

        // 3. Get specific request details
        [HttpGet("{id}")]
        public async Task<ActionResult> GetPatientRequestDetails(int id)
        {
            var patientId = GetPatientIdFromToken();
            var result = await _prescriptionRequestService.GetPatientRequestDetailsAsync(id, patientId);
            return Success(result, "Request details retrieved successfully");
        }
    }
}