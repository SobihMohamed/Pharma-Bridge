using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // عشان StatusCodes
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.PatientAddresses;
using PharmaBridge.Shared.DTOs.PatientAddresses;
using PharmaBridge.Shared.Common.Response; 
using PharmaBridge.Presentation.Controllers; 
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PharmaBridge.Controllers.PatientAddresses
{
    [ApiController]
    [Route("api/patient-addresses")]
    [Authorize(Roles = "Patient")] 
    public class PatientAddressesController : AppBaseController 
    {
        private readonly IPatientAddressService _addressService;

        public PatientAddressesController(IPatientAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PatientAddressDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> GetAddresses()
        {
            var patientId = GetCurrentPatientId();
            if (string.IsNullOrEmpty(patientId))
                return UnauthorizedError("User ID not found in token.");

            var addresses = await _addressService.GetPatientAddressesAsync(patientId);
            return Success(addresses, "Addresses retrieved successfully");
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PatientAddressDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddAddress([FromBody] CreatePatientAddressDto createDto)
        {
            var patientId = GetCurrentPatientId();
            if (string.IsNullOrEmpty(patientId))
                return UnauthorizedError("User ID not found in token.");

            var result = await _addressService.AddAddressAsync(patientId, createDto);
            return Created(result, "Address added successfully");
        }

        [HttpPut("{addressId}")]
        [ProducesResponseType(typeof(ApiResponse<PatientAddressDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateAddress(int addressId, [FromBody] UpdatePatientAddressDto updateDto)
        {
            var patientId = GetCurrentPatientId();
            if (string.IsNullOrEmpty(patientId))
                return UnauthorizedError("User ID not found in token.");

            var result = await _addressService.UpdateAddressAsync(addressId, patientId, updateDto);
            return Success(result, "Address updated successfully");
        }

        [HttpDelete("{addressId}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteAddress(int addressId)
        {
            var patientId = GetCurrentPatientId();
            if (string.IsNullOrEmpty(patientId))
                return UnauthorizedError("User ID not found in token.");

            var result = await _addressService.DeleteAddressAsync(addressId, patientId);
            return Success(result, "Address deleted successfully");
        }

        [HttpPut("{addressId}/set-default")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> SetDefaultAddress(int addressId)
        {
            var patientId = GetCurrentPatientId();
            if (string.IsNullOrEmpty(patientId))
                return UnauthorizedError("User ID not found in token.");

            var result = await _addressService.SetDefaultAddressAsync(addressId, patientId);
            return Success(result, "Default address set successfully");
        }

        #region Helper Method
        private string GetCurrentPatientId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        #endregion
    }
}