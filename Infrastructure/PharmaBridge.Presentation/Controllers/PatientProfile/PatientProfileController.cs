using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.CurrentUser;
using PharmaBridge.Abstraction.IServices.PatientProfiles;
using PharmaBridge.Shared.Common.Params.Patient;
using PharmaBridge.Shared.DTOs.PatientProfiles;
using System.Security.Claims;

namespace PharmaBridge.Presentation.Controllers.PatientProfile
{
    public class PatientProfileController : AppBaseController
    {
        private readonly IPatientProfileService _patientService;
        private readonly ICurrentUserService _currentUserService;

        public PatientProfileController(
            IPatientProfileService patientService,
            ICurrentUserService currentUserService)
        {
            _patientService = patientService;
            _currentUserService = currentUserService;
        }

        // 1. Get My Profile
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetMyProfile()
        {
            var userId = _currentUserService.UserId;

            var result = await _patientService.GetMyProfileAsync(userId);

            return Success(result, "Profile Retrieved Successfully");
        }

        // 2. Update My Profile
        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateMyProfile([FromBody] PatientProfileToUpdateDto dto)
        {
            var userId = _currentUserService.UserId;

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