using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Shared.DTOs.Pharmacy;

namespace PharmaBridge.Presentation.Controllers
{
    [Route("api/pharmacy")]
    [ApiController]
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyProfileService _pharmacyProfileService;

        public PharmacyController(IPharmacyProfileService pharmacyProfileService)
        {
            _pharmacyProfileService = pharmacyProfileService;
        }

        // POST /api/pharmacy/register
        [HttpPost("register")]
        [Authorize(Roles = "PharmacyOwner")]
        public async Task<IActionResult> RegisterPharmacyProfile([FromForm] PharmacyToCreateDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _pharmacyProfileService.RegisterPharmacyProfileAsync(createDto, userId);
            return Ok(result);
        }

        // GET /api/pharmacy/my-profile/{pharmacyId}
        [HttpGet("my-profile/{pharmacyId}")]
        [Authorize(Roles = "PharmacyOwner")]
        public async Task<IActionResult> GetMyProfile(int pharmacyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _pharmacyProfileService.GetMyProfileAsync(pharmacyId, userId);
            return Ok(result);
        }

        // PUT /api/pharmacy/my-profile/{pharmacyId}
        [HttpPut("my-profile/{pharmacyId}")]
        [Authorize(Roles = "PharmacyOwner")]
        public async Task<IActionResult> UpdateMyProfile(int pharmacyId, [FromForm] PharmacyToUpdateDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _pharmacyProfileService.UpdateMyProfileAsync(pharmacyId, updateDto, userId);
            return Ok(result);
        }

        // GET /api/pharmacy/{pharmacyId}
        [HttpGet("{pharmacyId}")]
        [Authorize(Roles = "Patient,PharmacyOwner")]
        public async Task<IActionResult> GetPharmacyBasicInfo(int pharmacyId)
        {
            var result = await _pharmacyProfileService.GetPharmacyBasicInfoAsync(pharmacyId);
            return Ok(result);
        }
    }
}
