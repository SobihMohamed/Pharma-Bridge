using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.PharmaOwnerProfiles;
using PharmaBridge.Shared.Common.Params.PharmaOwner;
using PharmaBridge.Shared.DTOs.PharmaOwners;
using PharmaBridge.Shared.Common.Response;
using System.Security.Claims;


namespace PharmaBridge.Presentation.Controllers.PharmaOwnerProfile
{
    [Route("api/pharma-owner-profile")]
    [ApiController]
    public class PharmaOwnerProfileController : AppBaseController
    {
        private readonly IPharmaOwnerProfileService _pharmaOwnerService;

        public PharmaOwnerProfileController(IPharmaOwnerProfileService pharmaOwnerService)
        {
            _pharmaOwnerService = pharmaOwnerService;
        }

        // Helper Method: Extracts the current logged-in user ID (ApplicationUserId) from JWT
        private string GetUserIdFromToken()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                throw new UnauthorizedAccessException("Invalid User ID in token.");
            return userIdStr;
        }

        // 1. Create My Profile
        [Authorize(Roles = "PharmacyOwner")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PharmaOwnerDetailsDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> CreateMyProfile([FromBody] PharmaOwnerToCreateDto dto)
        {
            var userId = GetUserIdFromToken();
            var result = await _pharmaOwnerService.CreateMyProfileAsync(userId, dto);
            return Created(result, "PharmaOwner Profile Created Successfully");
        }

        // 2. Get My Profile
        [Authorize(Roles = "PharmacyOwner")]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PharmaOwnerDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetMyProfile()
        {
            var userId = GetUserIdFromToken();
            var result = await _pharmaOwnerService.GetMyProfileAsync(userId);
            return Success(result, "Profile Retrieved Successfully");
        }

        // 3. Update My Profile
        [Authorize(Roles = "PharmacyOwner")]
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<PharmaOwnerDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateMyProfile([FromBody] PharmaOwnerToUpdateDto dto)
        {
            var userId = GetUserIdFromToken();
            var result = await _pharmaOwnerService.UpdateMyProfileAsync(userId, dto);
            return Success(result, "Profile Updated Successfully");
        }

        // 4. Get All PharmaOwners (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> GetAllPharmaOwners([FromQuery] PharmaOwnerQueryParams queryParams)
        {
            var result = await _pharmaOwnerService.GetAllPharmaOwnersAsync(queryParams);
            return Success(result, "PharmaOwners Retrieved Successfully");
        }

        // 5. Update PharmaOwner Status (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatePharmaOwnerStatus(string id, [FromQuery] PharmaBridge.Shared.EnumHelper.PharmaEnums.PharmaOwnerStatus status)
        {
            var result = await _pharmaOwnerService.UpdatePharmaOwnerStatusAsync(id, status);
            return Success(result, $"PharmaOwner Profile Status Successfully Updated to {status}");
        }
    }
}
