using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Order;
using PharmaBridge.Shared.Common.Params.Order;
using PharmaBridge.Shared.DTOs.Order;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Response;
using System;
using System.Security.Claims;

namespace PharmaBridge.Presentation.Controllers.Order
{
    [Route("api/orders")]
    [ApiController] 
    [Authorize]
    public class OrderController : AppBaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,PharmacyOwner")]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<OrderDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetOrders(
            [FromQuery] int pharmacyId,
            [FromQuery] OrderQueryParams queryParams)
        {
            var result = await _orderService.GetPharmacyOrdersAsync(pharmacyId, queryParams);
            return Success(result, "Orders retrieved successfully");
        }

        [HttpGet("{orderId:int}")]
        [Authorize(Roles = "Admin,PharmacyOwner")]
        [ProducesResponseType(typeof(ApiResponse<OrderDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderDetails(
            [FromRoute] int orderId, 
            [FromQuery] int pharmacyId)
        {
            var result = await _orderService.GetPharmacyOrderDetailsAsync(orderId, pharmacyId);
            return Success(result, "Order details retrieved successfully");
        }

        [HttpPatch("{orderId:int}/status")]
        [Authorize(Roles = "PharmacyOwner")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus(
            [FromRoute] int orderId,
            [FromBody] UpdateOrderStatusDto dto,
            [FromQuery] int pharmacyId)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, dto, pharmacyId);
            return Success(result, "Order status updated successfully");
        }

        // =====================================================================
        // PATIENT OPERATIONS — Phase 2
        // =====================================================================

        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<OrderDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyOrders([FromQuery] OrderQueryParams queryParams)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var patientId))
                return UnauthorizedError("Invalid Patient ID in token."); 

            var result = await _orderService.GetPatientOrdersAsync(patientId, queryParams);
            return Success(result, "Your orders retrieved successfully");
        }

        [HttpGet("my/{orderId:int}")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(typeof(ApiResponse<OrderDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyOrderDetails([FromRoute] int orderId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var patientId))
                return UnauthorizedError("Invalid Patient ID in token.");

            var result = await _orderService.GetPatientOrderDetailsAsync(orderId, patientId);
            return Success(result, "Order details retrieved successfully");
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<OrderDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllPlatformOrders(
            [FromQuery] OrderQueryParams queryParams)
        {
            var result = await _orderService.GetAllPlatformOrdersAsync(queryParams);
            return Success(result, "Platform orders retrieved successfully");
        }

        [HttpGet("admin/{orderId:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<OrderDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAdminOrderDetails([FromRoute] int orderId)
        {
            var result = await _orderService.GetAdminOrderDetailsAsync(orderId);
            return Success(result, "Order details retrieved successfully");
        }

        // Temp

        [AllowAnonymous]
        [HttpPost("create-from-bid/{bidId:int}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFromBid([FromRoute] int bidId)
        {
            var result = await _orderService.CreateOrderFromBidAsync(bidId);
            return Created(result, "Order created successfully");
        }
    }
}