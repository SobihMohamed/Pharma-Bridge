using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // عشان StatusCodes
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Complaint;
using PharmaBridge.Shared.Common.Params.Complaint;
using PharmaBridge.Shared.DTOs.Complaint;
using PharmaBridge.Shared.Common.Response; 
using PharmaBridge.Shared.Common.Pagination; 
using System.Security.Claims;

namespace PharmaBridge.Presentation.Controllers.Complaints
{
    [Route("api/complaints")] 
    [ApiController]
    public class ComplaintsController : AppBaseController 
    {
        private readonly IComplaintService _complaintService;

        public ComplaintsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        // --- Patient Endpoints ---

        [HttpPost("submit")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(typeof(ApiResponse<ComplaintDto>), StatusCodes.Status201Created)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SubmitComplaint([FromBody] CreateComplaintDto createDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid patientId))
                return UnauthorizedError("Invalid User Token.");

            var result = await _complaintService.SubmitComplaintAsync(createDto, patientId);
            return Created(result, "Complaint submitted successfully.");
        }

        [HttpGet("my-complaints")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<ComplaintDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPatientComplaints([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 1)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid patientId))
                return UnauthorizedError("Invalid User Token.");

            var result = await _complaintService.GetPatientComplaintsAsync(patientId, pageSize, pageIndex);
            return Success(result, "Patient complaints retrieved successfully.");
        }

        // --- Admin Operations ---

        [HttpGet("platform-complaints")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<ComplaintDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllPlatformComplaints([FromQuery] ComplaintQueryParams queryParams)
        {
            var result = await _complaintService.GetAllPlatformComplaintsAsync(queryParams);
            return Success(result, "Platform complaints retrieved successfully.");
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<ComplaintDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComplaintDetails(int id)
        {
            var result = await _complaintService.GetComplaintDetailsAsync(id);
            return Success(result, "Complaint details retrieved successfully.");
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComplaintStatus(int id, [FromBody] UpdateComplaintStatusDto updateDto)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(adminId))
                return UnauthorizedError("Admin context not found.");

            var result = await _complaintService.UpdateComplaintStatusAsync(id, updateDto, adminId);

            return Success(true, "Complaint status updated successfully.");
        }
    }
}