using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.PatientAddresses;
using PharmaBridge.Shared.DTOs.PatientAddresses;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PharmaBridge.Controllers.PatientAddresses
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientAddressesController : ControllerBase
    {
        private readonly IPatientAddressService _addressService;

        public PatientAddressesController(IPatientAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<PatientAddressDto>>> GetAddresses()
        {
            var patientId = GetCurrentPatientId();
            if (string.IsNullOrEmpty(patientId))
                return Unauthorized("User ID not found in token.");

            var addresses = await _addressService.GetPatientAddressesAsync(patientId);
            return Ok(addresses);
        }

        [HttpPost]
        public async Task<ActionResult<PatientAddressDto>> AddAddress([FromBody] CreatePatientAddressDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patientId = GetCurrentPatientId();
            if (string.IsNullOrEmpty(patientId))
                return Unauthorized();

            var result = await _addressService.AddAddressAsync(patientId, createDto);
            return Ok(result);
        }

        [HttpPut("{addressId}")]
        public async Task<ActionResult<PatientAddressDto>> UpdateAddress(int addressId, [FromBody] UpdatePatientAddressDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patientId = GetCurrentPatientId();
            if (string.IsNullOrEmpty(patientId))
                return Unauthorized();

            var result = await _addressService.UpdateAddressAsync(addressId, patientId, updateDto);
            return Ok(result);
        }

        [HttpDelete("{addressId}")]
        public async Task<ActionResult<bool>> DeleteAddress(int addressId)
        {
            var patientId = GetCurrentPatientId();
            if (string.IsNullOrEmpty(patientId))
                return Unauthorized();

            var result = await _addressService.DeleteAddressAsync(addressId, patientId);
            return Ok(result);
        }

        [HttpPut("{addressId}/set-default")]
        public async Task<ActionResult<bool>> SetDefaultAddress(int addressId)
        {
            var patientId = GetCurrentPatientId();
            if (string.IsNullOrEmpty(patientId))
                return Unauthorized();

            var result = await _addressService.SetDefaultAddressAsync(addressId, patientId);
            return Ok(result);
        }


        #region Helper Method
        private string GetCurrentPatientId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        #endregion
    }
}