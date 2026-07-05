using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Patient;
using PharmaBridge.Shared.Common.Params.Patient;
using PharmaBridge.Shared.Common.Response;
using PharmaBridge.Shared.DTOs.Patient;
using System.Security.Claims;

namespace PharmaBridge.Presentation.Controllers.Patient
{
    [Route("api/patient/home")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class PatientHomeController : AppBaseController
    {
        private readonly IPatientHomeService _patientHomeService;

        public PatientHomeController(IPatientHomeService patientHomeService)
        {
            _patientHomeService = patientHomeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PatientHomeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetHome([FromQuery] PatientHomeQueryParams queryParams)
        {
            var userId = GetUserIdFromToken();
            var result = await _patientHomeService.GetPatientHomeAsync(userId, queryParams);
            return Success(result, "Home data retrieved successfully");
        }

        private Guid GetUserIdFromToken()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedAccessException("Invalid User ID in token.");

            return userId;
        }
    }
}