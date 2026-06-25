using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Complaint;
using PharmaBridge.Shared.Common.Params.Complaint;
using PharmaBridge.Shared.DTOs.Complaint;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace PharmaBridge.Presentation.Controllers.Complaints
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaintService _complaintService;

        public ComplaintsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        //Patient Endpoints
        [HttpPost("submit")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> SubmitComplaint([FromBody] CreateComplaintDto createDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid patientId))
                return Unauthorized(new { Message = "Invalid User Token." });
            var result = await _complaintService.SubmitComplaintAsync(createDto, patientId);
            return Ok(result);
        }

        [HttpGet("my-complaints")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPatientComplaints([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 1)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid patientId))
                return Unauthorized(new { Message = "Invalid User Token." });
            var result = await _complaintService.GetPatientComplaintsAsync(patientId, pageSize, pageIndex);
            return Ok(result);
        }

        // --- Admin Operations ---
        [HttpGet("platform-complaints")]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPlatformComplaints([FromQuery] ComplaintQueryParams queryParams)
        {
            var result = await _complaintService.GetAllPlatformComplaintsAsync(queryParams);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetComplaintDetails(int id)
        {
            var result = await _complaintService.GetComplaintDetailsAsync(id);
            return Ok(result);
        }

        [HttpPut("{id:int}/status")]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateComplaintStatus(int id, [FromBody] UpdateComplaintStatusDto updateDto)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized(new { Message = "Admin context not found." });
            var result = await _complaintService.UpdateComplaintStatusAsync(id, updateDto, adminId);
            return Ok(new { Message = "Complaint status updated successfully." });
        }
    }
}