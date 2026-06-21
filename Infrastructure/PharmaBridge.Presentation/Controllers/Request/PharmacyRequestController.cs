using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Pharmacy; 
using PharmaBridge.Abstraction.IServices.PrescriptionRequest;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using System;
using System.Security.Claims;

namespace PharmaBridge.Presentation.Controllers
{
    [Route("api/pharmacy-requests")]
    [Authorize(Roles = "PharmacyOwner")]
    public class PharmacyRequestController : AppBaseController
    {
        private readonly IPrescriptionRequestService _prescriptionRequestService;
        private readonly IPharmacyProfileService _pharmacyProfileService; 

        public PharmacyRequestController(
            IPrescriptionRequestService prescriptionRequestService,
            IPharmacyProfileService pharmacyProfileService)
        {
            _prescriptionRequestService = prescriptionRequestService;
            _pharmacyProfileService = pharmacyProfileService;
        }

        private string GetUserIdFromToken()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid User ID in token.");
            return userId;
        }

        [HttpGet("nearby")]
        public async Task<ActionResult> GetNearbyRequests([FromQuery] PrescriptionRequestQueryParams queryParams)
        {
            var userId = GetUserIdFromToken();

            int pharmacyId = await _pharmacyProfileService.GetPharmacyIdByUserIdAsync(userId);

            var result = await _prescriptionRequestService.GetNearbyRequestsAsync(pharmacyId, queryParams);

            return Success(result, "Nearby requests retrieved successfully");
        }
    }
}