using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.PatientProfiles;
using PharmaBridge.Shared.Common.Params.Patient;
using PharmaBridge.Shared.DTOs.PatientProfiles;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PharmaBridge.Presentation.Controllers.PatientProfile
{
    [Route("api/patient-profile")]
    [ApiController]
    public class PatientProfileController : AppBaseController
    {
        private readonly IPatientProfileService _patientService;

        public PatientProfileController(IPatientProfileService patientService)
        {
            _patientService = patientService;
        }

        // Helper Method: Extracts the current logged-in user ID (ApplicationUserId) from JWT
        private string GetUserIdFromToken()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                throw new UnauthorizedAccessException("Invalid User ID in token.");
            return userIdStr;
        }

        // 1. Get My Profile
        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<ActionResult> GetMyProfile()
        {
            var userId = GetUserIdFromToken();
            var result = await _patientService.GetMyProfileAsync(userId);
            return Success(result, "Profile Retrieved Successfully");
        }

        // 2. Update My Profile
        [Authorize(Roles = "Patient")]
        [HttpPut]
        public async Task<ActionResult> UpdateMyProfile([FromBody] PatientProfileToUpdateDto dto)
        {
            var userId = GetUserIdFromToken();
            var result = await _patientService.UpdateMyProfileAsync(userId, dto);
            return Success(result, "Profile Updated Successfully");
        }

        // 3. Get All Patients (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult> GetAllPatients([FromQuery] PatientQueryParams queryParams)
        {
            var result = await _patientService.GetAllPatientsAsync(queryParams);
            return Success(result, "Patients Retrieved Successfully");
        }
    }
}