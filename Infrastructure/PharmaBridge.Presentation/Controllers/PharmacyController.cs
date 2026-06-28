using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Shared.Common.Response; 
using PharmaBridge.Shared.DTOs.Pharmacy;
using PharmaBridge.Shared.DTOs.PharmaPerformSnapshot;
using System.Security.Claims;

namespace PharmaBridge.Presentation.Controllers
{
    [Route("api/pharmacy")]
    [ApiController]
    public class PharmacyController : AppBaseController
    {
        private readonly IPharmacyProfileService _pharmacyProfileService;

        public PharmacyController(IPharmacyProfileService pharmacyProfileService)
        {
            _pharmacyProfileService = pharmacyProfileService;
        }

        // POST /api/pharmacy/register
        [HttpPost("register")]
        [Authorize(Roles = "PharmacyOwner")]
        [Consumes("multipart/form-data")] 
        [ProducesResponseType(typeof(ApiResponse<PharmacyOwnerProfileDto>), StatusCodes.Status201Created)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterPharmacyProfile([FromForm] PharmacyToCreateDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedError();

            var result = await _pharmacyProfileService.RegisterPharmacyProfileAsync(createDto, userId);

            return Created(result, "The pharmacy has been successfully registered and is now under review.");
        }

        // GET /api/pharmacy/my-profile/{pharmacyId}
        [HttpGet("my-profile/{pharmacyId}")]
        [Authorize(Roles = "PharmacyOwner")]
        [ProducesResponseType(typeof(ApiResponse<PharmacyOwnerProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile(int pharmacyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedError();

            var result = await _pharmacyProfileService.GetMyProfileAsync(pharmacyId, userId);

            return Success(result);
        }

        // PATCH /api/pharmacy/my-profile/{pharmacyId}
        [HttpPatch("my-profile/{pharmacyId}")]
        [Authorize(Roles = "PharmacyOwner")]
        [Consumes("multipart/form-data")] 
        [ProducesResponseType(typeof(ApiResponse<PharmacyOwnerProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMyProfile(int pharmacyId, [FromForm] PharmacyToUpdateDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return UnauthorizedError();

            var result = await _pharmacyProfileService.UpdateMyProfileAsync(pharmacyId, updateDto, userId);

            return Success(result, "The pharmacy data has been successfully updated.");
        }

        // GET /api/pharmacy/{pharmacyId}
        [HttpGet("{pharmacyId}")]
        [Authorize(Roles = "Admin,Patient,PharmacyOwner")]
        [ProducesResponseType(typeof(ApiResponse<PharmacyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPharmacyBasicInfo(int pharmacyId)
        {
            var result = await _pharmacyProfileService.GetPharmacyBasicInfoAsync(pharmacyId);
            return Success(result);
        }

        // GET /api/pharmacy/{pharmacyId}/dashboard/snapshot
        [HttpGet("{pharmacyId}/dashboard/snapshot")]
        [Authorize(Roles = "Admin,PharmacyOwner")]
        [ProducesResponseType(typeof(ApiResponse<PharmaPerformSnapshotDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPharmacyPerformanceSnapshot(
            int pharmacyId,
            [FromServices] IPharmacyDashboardService pharmacyDashboardService)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                return UnauthorizedError();

            var result = await pharmacyDashboardService.GetMyPerformanceSnapshotAsync(pharmacyId, userId, role);
            return Success(result);
        }
    }
}