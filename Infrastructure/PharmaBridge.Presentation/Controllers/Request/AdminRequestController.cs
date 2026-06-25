using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.PrescriptionRequest;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using PharmaBridge.Shared.Common.Response; 
using PharmaBridge.Shared.Common.Pagination; 

namespace PharmaBridge.Presentation.Controllers
{
    [Route("api/admin/prescription-requests")]
    [Authorize(Roles = "Admin")]
    public class AdminRequestController : AppBaseController
    {
        private readonly IPrescriptionRequestService _prescriptionRequestService;

        public AdminRequestController(IPrescriptionRequestService prescriptionRequestService)
        {
            _prescriptionRequestService = prescriptionRequestService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<object>>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> GetAllPlatformRequests([FromQuery] PrescriptionRequestQueryParams queryParams)
        {
            var result = await _prescriptionRequestService.GetAllPlatformRequestsAsync(queryParams);
            return Success(result, "All platform requests retrieved successfully");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAdminRequestDetails(int id)
        {
            var result = await _prescriptionRequestService.GetAdminRequestDetailsAsync(id);
            return Success(result, "Admin request details retrieved successfully");
        }
    }
}