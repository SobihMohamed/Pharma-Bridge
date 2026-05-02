using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Order;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Shared.Common.Params.Order;
using PharmaBridge.Shared.DTOs.Order;
using System.Security.Claims;

namespace PharmaBridge.Presentation.Controllers.Order
{
    [Route("api/orders")]
    [Authorize]
    public class OrderController(IOrderService orderService) : AppBaseController
    {

        private Guid GetPatientIdFromToken()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var patientId))
                throw new UnauthorizedAccessException("Invalid Patient ID in token.");
            return patientId;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,PharmacyOwner")]
        public async Task<IActionResult> GetOrders(
            [FromQuery] int pharmacyId,
            [FromQuery] OrderQueryParams queryParams)
        {
            var result = await orderService.GetPharmacyOrdersAsync(pharmacyId, queryParams);
            return Success(result, "Orders retrieved successfully");
        }

        [HttpGet("{orderId}")]
        [Authorize(Roles = "Admin,PharmacyOwner")]
        public async Task<IActionResult> GetOrderDetails(
            int orderId,
            [FromQuery] int pharmacyId)
        {
            var result = await orderService.GetPharmacyOrderDetailsAsync(orderId, pharmacyId);
            return Success(result, "Order details retrieved successfully");
        }

        [HttpPatch("{orderId}/status")]
        [Authorize(Roles = "PharmacyOwner")]
        public async Task<IActionResult> UpdateOrderStatus(
            int orderId,
            [FromBody] UpdateOrderStatusDto dto,
            [FromQuery] int pharmacyId)
        {
            var result = await orderService.UpdateOrderStatusAsync(orderId, dto, pharmacyId);
            return Success(result, "Order status updated successfully");
        }
        // =====================================================================
        // PATIENT OPERATIONS — Phase 2
        // =====================================================================

        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyOrders([FromQuery] OrderQueryParams queryParams)
        {
            var patientId = GetPatientIdFromToken();
            var result = await orderService.GetPatientOrdersAsync(patientId, queryParams);
            return Success(result, "Your orders retrieved successfully");
        }

        [HttpGet("my/{orderId}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyOrderDetails(int orderId)
        {
            var patientId = GetPatientIdFromToken();
            var result = await orderService.GetPatientOrderDetailsAsync(orderId, patientId);
            return Success(result, "Order details retrieved successfully");
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPlatformOrders(
            [FromQuery] OrderQueryParams queryParams)
        {
            var result = await orderService.GetAllPlatformOrdersAsync(queryParams);
            return Success(result, "Platform orders retrieved successfully");
        }

        [HttpGet("admin/{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminOrderDetails(int orderId)
        {
            var result = await orderService.GetAdminOrderDetailsAsync(orderId);
            return Success(result, "Order details retrieved successfully");
        }

        // Temp

        [AllowAnonymous]
        [HttpPost("create-from-bid/{bidId}")]
        public async Task<IActionResult> CreateFromBid(int bidId)
        {
            var result = await orderService.CreateOrderFromBidAsync(bidId);
            return Created(result, "Order created successfully");
        }
    }
}