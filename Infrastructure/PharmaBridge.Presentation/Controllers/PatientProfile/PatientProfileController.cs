using PharmaBridge.Abstraction.IServices.PatientProfiles;
using PharmaBridge.Shared.Common.Params.Patient;
using PharmaBridge.Shared.DTOs.PatientProfiles;
using Microsoft.AspNetCore.Mvc;

namespace PharmaBridge.Presentation.Controllers.PatientProfile
{
    public class PatientProfileController : AppBaseController
    {
        private readonly IPatientProfileService _patientService;

        public PatientProfileController(IPatientProfileService patientService)
        {
            _patientService = patientService;
        }

        // 1. Get My Profile
        [HttpGet("{patientId}")]
        public async Task<ActionResult> GetMyProfile(string patientId)
        {
            var result = await _patientService.GetMyProfileAsync(patientId);
            return Success(result, "Profile Retrieved Successfully");
        }

        // 2. Update My Profile
        [HttpPut("{patientId}")]
        public async Task<ActionResult> UpdateMyProfile(string patientId, [FromBody] PatientProfileToUpdateDto dto)
        {
            var result = await _patientService.UpdateMyProfileAsync(patientId, dto);
            return Success(result, "Profile Updated Successfully");
        }

        // 3. Get All Patients (Admin)
        [HttpGet]
        public async Task<ActionResult> GetAllPatients([FromQuery] PatientQueryParams queryParams)
        {
            var result = await _patientService.GetAllPatientsAsync(queryParams);
            return Success(result, "Patients Retrieved Successfully");
        }
    }
}